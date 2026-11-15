using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.Difficulty;

public class DifficultyAutoTagger : AutoTagger<TimestampLevelIdCursor>
{
    private static readonly HashSet<string> DifficultyTagNames =
    [
        "casual",
        "normal",
        "advanced",
        "expert",
        "nightmare",
        "brutal",
        "lunatic",
        "insane",
        "impossible",
    ];

    private static readonly Dictionary<string, string> DifficultyMappings = new Dictionary<string, string>()
    {
        { "Casual", "casual" },
        { "Normal", "normal" },
        { "Advanced", "advanced" },
        { "Expert", "expert" },
        // We don't apply nightmare difficulty because we don't know if the level might be even more difficult than nightmare.
        //{ "Nightmare", "nightmare" },
    };

    private readonly UpdatedWorkshopLevelsTaggingStrategy _TaggingStrategy = new UpdatedWorkshopLevelsTaggingStrategy();

    public override Guid Guid { get; } = new Guid("2F5E210F-22C8-4BD3-B2C3-35655E3EC8F0");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(5);

    public override AutoTaggerLevelsQuery GetLevelsQuery()
    {
        return _TaggingStrategy.GetLevelsQuery(State);
    }

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        TagDelta tagDelta = new TagDelta();
        if (level is not WorkshopLevelDto workshopLevel)
            return Task.FromResult(tagDelta);

        // Do not change any existing difficulty tags.
        if (level.Tags.Any(tagName => DifficultyTagNames.Contains(tagName)))
            return Task.FromResult(tagDelta);

        string? workshopDifficultyTagName = workshopLevel.WorkshopTags
            .Join(
                DifficultyMappings,
                workshopTag => workshopTag,
                difficultyMapping => difficultyMapping.Key,
                (_, difficultyMapping) => difficultyMapping.Value)
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(workshopDifficultyTagName))
            tagDelta.AddTags.Add(workshopDifficultyTagName);

        return Task.FromResult(tagDelta);
    }

    public override void UpdateState(LevelDto level, TagDelta calculatedTagDelta)
    {
        State = _TaggingStrategy.UpdateCursor(State, level);
    }
}
