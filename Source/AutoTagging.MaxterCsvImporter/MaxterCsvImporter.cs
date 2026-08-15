using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core.Mapping;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvImporter : AutoTagger
{
    private static readonly string PluginDirectory =
        Path.GetDirectoryName(typeof(MaxterCsvImporter).Assembly.Location)
        ?? AppContext.BaseDirectory;

    private readonly Lazy<MaxterCsvRepository> _CsvRepository = new Lazy<MaxterCsvRepository>(
        () => new MaxterCsvRepository(Path.Combine(PluginDirectory, "maxter_tagging_all_levels.csv")));

    private readonly Lazy<MaxterTagMappingData> _Mapping = new Lazy<MaxterTagMappingData>(
        () => MaxterTagMappingLoader.Load(Path.Combine(PluginDirectory, MaxterTagMappingLoader.MappingFileName)));

    private readonly Lazy<AppliedTagsLedger> _Ledger = new Lazy<AppliedTagsLedger>(
        () => new AppliedTagsLedger(Path.Combine(PluginDirectory, $"{nameof(MaxterCsvImporter)}.AppliedTags.json")));

    public override TaggingStrategy TaggingStrategy => TaggingStrategy.AllLevels;

    public override LevelProjectionIncludes RequiredIncludes => LevelProjectionIncludes.Tags;

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelProjectionDto level, CancellationToken cancellationToken = default)
    {
        if (level.Id == null || level.Tags == null)
            throw new ArgumentException("Required properties of level not initialized.", nameof(level));

        MaxterCsvRow? row = _CsvRepository.Value.FindRow(level.Id.Value);
        if (row == null)
            return Task.FromResult(new TagDelta());

        HashSet<string> desired = MaxterTagResolver.ResolveDesiredTags(row, _Mapping.Value);
        IReadOnlyCollection<string> previouslyOurs = _Ledger.Value.GetPreviouslyApplied(level.Id.Value);
        HashSet<string> current = level.Tags.ToHashSet();

        TagDelta tagDelta = new TagDelta
        {
            // Only ever remove a tag this importer itself recorded adding before - never a tag
            // owned by another auto-tagger or set by a human curator, even if the name matches.
            AddTags = desired.Except(current).ToList(),
            RemoveTags = previouslyOurs.Except(desired).Intersect(current).ToList(),
        };

        _Ledger.Value.SetCurrentlyApplied(level.Id.Value, desired);

        return Task.FromResult(tagDelta);
    }
}
