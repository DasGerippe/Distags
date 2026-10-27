using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.LevelType;

public class LevelTypeAutoTagger : AutoTagger<ProgressState>
{
    private const string OfficialTagName = "official";
    private const string WorkshopTagName = "workshop";

    public override Guid Guid { get; } = new Guid("2E6677C5-DF38-498A-843E-0468B21CFF94");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(5);

    public override ITaggingStrategy TaggingStrategy { get; } = new NewLevelsTaggingStrategy();

    public override AutoTaggerLevelsQuery GetLevelsQuery()
    {
        return TaggingStrategy.GetLevelsQuery(State?.Progress);
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
        State ??= new ProgressState();
        State.Progress = TaggingStrategy.UpdateProgress(State.Progress, level);
    }
}
