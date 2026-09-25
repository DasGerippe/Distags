using Distags.Contracts;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class NewWorkshopLevelsTaggingStrategy : ProgressTrackingTaggingStrategy
{
    public NewWorkshopLevelsTaggingStrategy()
        : base(level => level.RecordCreatedOn, LevelSortStrategy.RecordCreatedAsc, LevelType.Workshop)
    {
    }
}
