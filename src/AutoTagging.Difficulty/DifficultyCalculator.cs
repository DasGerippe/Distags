using AutoTagging.Abstractions;
using Distags.Contracts;

namespace AutoTagging.Difficulty;

public class DifficultyCalculator : ITagDeltaCalculator
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

    public Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
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
}
