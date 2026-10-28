using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.MissingLevel;

public class MissingLevelAutoTagger : AutoTagger<TimestampLevelIdCursor>
{
    private const string MissingLevelTag = "missing-level";

    private readonly NewWorkshopLevelsTaggingStrategy _TaggingStrategy = new NewWorkshopLevelsTaggingStrategy();

    public override Guid Guid { get; } = new Guid("074598C5-1C0A-4302-95C4-F93B5572E399");

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

        if (DistanceFileNameValidator.IsValidFileName(workshopLevel.FileName))
        {
            if (level.Tags.Contains(MissingLevelTag))
                tagDelta.RemoveTags.Add(MissingLevelTag);
        }
        else
        {
            if (!level.Tags.Contains(MissingLevelTag))
                tagDelta.AddTags.Add(MissingLevelTag);
        }

        return Task.FromResult(tagDelta);
    }

    public override void UpdateState(LevelDto level, TagDelta tagDelta)
    {
        State = _TaggingStrategy.UpdateCursor(State, level);
    }
}
