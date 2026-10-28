using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.GameMode;

public class GameModeAutoTagger : AutoTagger<TimestampLevelIdCursor>
{
    private static readonly Dictionary<string, string> GameModeTagNames = new Dictionary<string, string>
    {
        { "Sprint", "sprint" },
        { "Challenge", "challenge" },
        { "Stunt" , "stunt" },
        { "Reverse Tag", "reverse-tag" },
        { "Main Menu", "main-menu" },
    };

    private readonly UpdatedLevelsTaggingStrategy _TaggingStrategy = new UpdatedLevelsTaggingStrategy();

    public override Guid Guid { get; } = new Guid("AD96F858-91F3-46E7-B2C2-19DAFBA0DFDF");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(5);

    public override AutoTaggerLevelsQuery GetLevelsQuery()
    {
        return _TaggingStrategy.GetLevelsQuery(State);
    }

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
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

    public override void UpdateState(LevelDto level, TagDelta tagDelta)
    {
        State = _TaggingStrategy.UpdateCursor(State, level);
    }
}
