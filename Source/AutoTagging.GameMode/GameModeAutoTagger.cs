using AutoTagging.Abstractions;
using DataClasses;

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

    public override Task<TagDelta> CalculateTagDeltaAsync(Level level, CancellationToken cancellationToken = default)
    {
        List<string> currentGameModeTags = level.Tags
            .Where(tag => GameModeTags.Values.Contains(tag.Name))
            .Select(tag => tag.Name)
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