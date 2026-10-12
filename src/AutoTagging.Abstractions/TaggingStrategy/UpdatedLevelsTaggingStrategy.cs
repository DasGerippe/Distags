using Distags.Contracts;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class UpdatedLevelsTaggingStrategy : ProgressTrackingTaggingStrategy
{
    public UpdatedLevelsTaggingStrategy()
        : base(level => level.RecordUpdatedOn, LevelSortStrategy.RecordUpdatedAsc)
    {
    }
}
