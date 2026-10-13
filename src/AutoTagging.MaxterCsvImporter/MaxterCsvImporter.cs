using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Microsoft.Extensions.Logging;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvImporter : AutoTagger
{
    private static readonly string PluginDirectory =
        Path.GetDirectoryName(typeof(MaxterCsvImporter).Assembly.Location)
        ?? AppContext.BaseDirectory;

    private readonly MaxterCsvRepository _CsvRepository;
    private readonly Lazy<MaxterTagMappingData> _TagMapping;
    private readonly MaxterTagResolver _TagResolver;
    private readonly AppliedTagsLedger _Ledger;

    public override ITaggingStrategy TaggingStrategy { get; } = new AllLevelsTaggingStrategy();

    public MaxterCsvImporter(ILoggerFactory loggerFactory)
    {
        _CsvRepository = new MaxterCsvRepository(
            Path.Combine(PluginDirectory, "maxter_tagging_all_levels.csv"),
            loggerFactory.CreateLogger<MaxterCsvRepository>());

        MaxterTagMappingLoader tagMappingLoader = new MaxterTagMappingLoader(loggerFactory.CreateLogger<MaxterTagMappingLoader>());
        _TagMapping = new Lazy<MaxterTagMappingData>(
            tagMappingLoader.Load(Path.Combine(PluginDirectory, MaxterTagMappingLoader.MappingFileName)));

        _TagResolver = new MaxterTagResolver(loggerFactory.CreateLogger<MaxterTagResolver>());

        _Ledger = new AppliedTagsLedger(
            Path.Combine(PluginDirectory, $"{nameof(MaxterCsvImporter)}.AppliedTags.json"),
            loggerFactory.CreateLogger<AppliedTagsLedger>());
    }

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        MaxterCsvRow? row = _CsvRepository.FindRow(level);
        if (row == null)
            return Task.FromResult(new TagDelta());

        HashSet<string> desired = _TagResolver.ResolveDesiredTags(row, _TagMapping.Value);
        IReadOnlyCollection<string> previouslyOurs = _Ledger.GetPreviouslyApplied(level);
        HashSet<string> current = level.Tags.ToHashSet();

        TagDelta tagDelta = new TagDelta
        {
            // Only ever remove a tag this importer itself recorded adding before - never a tag
            // owned by another auto-tagger or set by a human curator, even if the name matches.
            AddTags = desired.Except(current).ToList(),
            RemoveTags = previouslyOurs.Except(desired).Intersect(current).ToList(),
        };

        _Ledger.SetCurrentlyApplied(level, desired);

        return Task.FromResult(tagDelta);
    }
}
