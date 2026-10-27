using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvImporter : AutoTagger<MaxterCsvImporterState>
{
    private readonly MaxterCsvRepository _CsvRepository;
    private readonly Lazy<MaxterTagMappingData> _TagMapping;
    private readonly MaxterTagResolver _TagResolver;

    private readonly string _BaseDirectory = Path.GetDirectoryName(typeof(MaxterCsvImporter).Assembly.Location) ?? AppContext.BaseDirectory;

    public override Guid Guid { get; } = new Guid("7BD71F81-FF4E-4D41-9A3D-510B82E3E7AC");

    public override TimeSpan? Interval => null;

    public MaxterCsvImporter(
        MaxterCsvRepositoryFactory maxterCsvRepositoryFactory,
        MaxterTagMappingLoader tagMappingLoader,
        MaxterTagResolver tagResolver)
    {
        string csvFilePath = Path.Combine(_BaseDirectory, Constants.TaggingCsvFileName);
        _CsvRepository = maxterCsvRepositoryFactory.CreateRepository(csvFilePath);

        string mappingFilePath = Path.Combine(_BaseDirectory, Constants.TagMappingJsonFileName);
        _TagMapping = new Lazy<MaxterTagMappingData>(() => tagMappingLoader.Load(mappingFilePath));

        _TagResolver = tagResolver;
    }

    public override AutoTaggerLevelsQuery GetLevelsQuery()
    {
        return TaggingStrategy.GetLevelsQuery(null);
    }

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        MaxterCsvRow? row = _CsvRepository.FindRow(level);
        if (row == null)
            return Task.FromResult(new TagDelta());

        HashSet<string> desired = _TagResolver.ResolveDesiredTags(row, _TagMapping.Value);
        List<string> previouslyAdded = State?.AppliedTagsLog.GetValueOrDefault(level.Id) ?? [];
        HashSet<string> current = level.Tags.ToHashSet();

        TagDelta tagDelta = new TagDelta
        {
            // Only ever remove a tag this importer itself recorded adding before - never a tag
            // owned by another auto-tagger or set by a human curator, even if the name matches.
            AddTags = desired.Except(current).ToList(),
            RemoveTags = current.Except(desired).Intersect(previouslyAdded).ToList(),
        };

        return Task.FromResult(tagDelta);
    }

    public override void UpdateState(LevelDto level, TagDelta tagDelta)
    {
        State ??= new MaxterCsvImporterState();

        long levelId = level.Id;
        List<string> oldAppliedLevelTags = State.AppliedTagsLog.GetValueOrDefault(levelId, []);
        List<string> newAppliedLevelTags = oldAppliedLevelTags
            .Except(tagDelta.RemoveTags)
            .Concat(tagDelta.AddTags)
            .ToList();

        if (newAppliedLevelTags.Count == 0)
            State.AppliedTagsLog.Remove(levelId);
        else
            State.AppliedTagsLog[levelId] = newAppliedLevelTags;
    }
}
