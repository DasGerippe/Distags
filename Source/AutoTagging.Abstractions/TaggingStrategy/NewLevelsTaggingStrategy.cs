using Distags.Contracts;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class NewLevelsTaggingStrategy : ProgressTrackingTaggingStrategy
{
    public NewLevelsTaggingStrategy()
        : base(level => level.RecordCreatedOn, LevelSortStrategy.RecordCreatedAsc)
    {
    }
}
