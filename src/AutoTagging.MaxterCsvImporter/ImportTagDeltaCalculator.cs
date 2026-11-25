using AutoTagging.Abstractions;
using Distags.Contracts;

namespace AutoTagging.MaxterCsvImporter;

public class ImportTagDeltaCalculator
{
    private readonly MaxterCsvRepository _CsvRepository;
    private readonly Lazy<MaxterTagMappingData> _TagMapping;
    private readonly MaxterTagResolver _TagResolver;

    private readonly string _BaseDirectory = Path.GetDirectoryName(typeof(MaxterCsvImporter).Assembly.Location) ?? AppContext.BaseDirectory;

    public ImportTagDeltaCalculator(
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

    public TagDelta CalculateTagDelta(LevelDto level, IReadOnlyDictionary<long, List<string>> appliedTagsLog)
    {
        MaxterCsvRow? row = _CsvRepository.FindRow(level);
        if (row == null)
            return new TagDelta();

        HashSet<string> desired = _TagResolver.ResolveDesiredTags(row, _TagMapping.Value);
        List<string> previouslyAdded = appliedTagsLog.GetValueOrDefault(level.Id) ?? [];
        HashSet<string> current = level.Tags.ToHashSet();

        TagDelta tagDelta = new TagDelta
        {
            // Only ever remove a tag this importer itself recorded adding before - never a tag
            // owned by another auto-tagger or set by a human curator, even if the name matches.
            AddTags = desired.Except(current).ToList(),
            RemoveTags = current.Except(desired).Intersect(previouslyAdded).ToList(),
        };

        return tagDelta;
    }
}
