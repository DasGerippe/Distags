using Distags.Contracts;

namespace AutoTagging.Abstractions.Progress;

public class UpdatedWorkshopLevelsStrategy : CursorProgressStrategy
{
    public UpdatedWorkshopLevelsStrategy()
        : base(level => level.RecordUpdatedOn, LevelSortStrategy.RecordUpdatedAsc, LevelType.Workshop)
    {
    }
}
