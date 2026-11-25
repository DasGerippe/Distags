using Distags.Contracts;

namespace AutoTagging.Abstractions.Progress;

public class UpdatedLevelsStrategy : CursorProgressStrategy
{
    public UpdatedLevelsStrategy()
        : base(level => level.RecordUpdatedOn, LevelSortStrategy.RecordUpdatedAsc)
    {
    }
}
