using Distags.Contracts;

namespace AutoTagging.Abstractions.Progress;

public class NewWorkshopLevelsStrategy : CursorProgressStrategy
{
    public NewWorkshopLevelsStrategy()
        : base(level => level.RecordCreatedOn, LevelSortStrategy.RecordCreatedAsc, LevelType.Workshop)
    {
    }
}
