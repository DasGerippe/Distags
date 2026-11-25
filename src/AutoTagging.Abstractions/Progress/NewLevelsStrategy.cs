using Distags.Contracts;

namespace AutoTagging.Abstractions.Progress;

public class NewLevelsStrategy : CursorProgressStrategy
{
    public NewLevelsStrategy()
        : base(level => level.RecordCreatedOn, LevelSortStrategy.RecordCreatedAsc)
    {
    }
}
