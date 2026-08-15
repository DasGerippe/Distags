using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core.Mapping;

namespace AutoTagging.GameMode;

public class GameModeAutoTagger : AutoTagger
{
    private static readonly Dictionary<string, string> GameModeTags = new Dictionary<string, string>
    {
        { "Sprint", "sprint" },
        { "Challenge", "challenge" },
        { "Stunt" , "stunt" },
        { "Reverse Tag", "reverse-tag" },
        { "Main Menu", "main-menu" },
    };

    public override TaggingStrategy TaggingStrategy => TaggingStrategy.NewAndUpdatedLevels;

    public override LevelProjectionIncludes RequiredIncludes =>
        LevelProjectionIncludes.WorkshopTags |
        LevelProjectionIncludes.Tags;

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelProjectionDto level, CancellationToken cancellationToken = default)
    {
        if (level.WorkshopTags == null || level.Tags == null)
            throw new ArgumentException("Required properties of level not initialized.", nameof(level));

        List<string> currentGameModeTags = level.Tags
            .Where(tag => GameModeTags.ContainsValue(tag))
            .Select(tag => tag)
            .ToList();

        IEnumerable<string> desiredGameModeTags = GameModeTags
            .Join(
                level.WorkshopTags,
                gameModeKvp => gameModeKvp.Key,
                workshopTag => workshopTag,
                (gameModeKvp, _) => gameModeKvp.Value)
            .ToList();

        TagDelta tagDelta = new TagDelta
        {
            AddTags = desiredGameModeTags.Except(currentGameModeTags).ToList(),
            RemoveTags = currentGameModeTags.Except(desiredGameModeTags).ToList(),
        };

        return Task.FromResult(tagDelta);
    }
}
