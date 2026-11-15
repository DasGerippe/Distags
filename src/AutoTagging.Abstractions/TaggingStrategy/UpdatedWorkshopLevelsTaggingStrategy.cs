using Distags.Contracts;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class UpdatedWorkshopLevelsTaggingStrategy : CursorTaggingStrategy
{
    public UpdatedWorkshopLevelsTaggingStrategy()
        : base(level => level.RecordUpdatedOn, LevelSortStrategy.RecordUpdatedAsc, LevelType.Workshop)
    {
    }
}
