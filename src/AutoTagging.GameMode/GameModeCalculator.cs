using AutoTagging.Abstractions;
using Distags.Contracts;

namespace AutoTagging.GameMode;

public class GameModeCalculator : ITagDeltaCalculator
{
    private static readonly Dictionary<string, string> GameModeTagNames = new Dictionary<string, string>
    {
        { "Sprint", "sprint" },
        { "Challenge", "challenge" },
        { "Stunt" , "stunt" },
        { "Reverse Tag", "reverse-tag" },
        { "Main Menu", "main-menu" },
    };

    public Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        List<string> levelGameModeNames = level switch
        {
            OfficialLevelDto officialLevel => officialLevel.GameModes,
            WorkshopLevelDto workshopLevel => workshopLevel.WorkshopTags,
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };

        List<string> currentGameModeTags = level.Tags
            .Where(tag => GameModeTagNames.ContainsValue(tag))
            .Select(tag => tag)
            .ToList();

        IEnumerable<string> desiredGameModeTags = GameModeTagNames
            .Join(
                levelGameModeNames,
                gameModeKvp => gameModeKvp.Key,
                levelGameModeName => levelGameModeName,
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
