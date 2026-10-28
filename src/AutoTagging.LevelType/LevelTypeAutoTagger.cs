using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.LevelType;

public class LevelTypeAutoTagger : AutoTagger<TimestampLevelIdCursor>
{
    private const string OfficialTagName = "official";
    private const string WorkshopTagName = "workshop";

    private readonly NewLevelsTaggingStrategy _TaggingStrategy = new NewLevelsTaggingStrategy();

    public override Guid Guid { get; } = new Guid("2E6677C5-DF38-498A-843E-0468B21CFF94");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(5);

    public override AutoTaggerLevelsQuery GetLevelsQuery()
    {
        return _TaggingStrategy.GetLevelsQuery(State);
    }

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        TagDelta tagDelta = new TagDelta();

        string desiredLevelTypeTagName = level.Type switch
        {
            Distags.Contracts.LevelType.Official => OfficialTagName,
            Distags.Contracts.LevelType.Workshop => WorkshopTagName,
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };

        if (!level.Tags.Contains(desiredLevelTypeTagName))
            tagDelta.AddTags.Add(desiredLevelTypeTagName);

        return Task.FromResult(tagDelta);
    }

    public override void UpdateState(LevelDto level, TagDelta tagDelta)
    {
        State = _TaggingStrategy.UpdateCursor(State, level);
    }
}
