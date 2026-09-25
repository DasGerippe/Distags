using Distags.Contracts;

namespace Distags.Core.Sorting.Levels;

public static class LevelSortStrategyExtensions
{
    public static SortOrder GetSortOrder(this LevelSortStrategy sortStrategy)
    {
        return sortStrategy switch
        {
            LevelSortStrategy.CreatedAsc => SortOrder.Ascending,
            LevelSortStrategy.CreatedDesc => SortOrder.Descending,
            LevelSortStrategy.UpdatedAsc => SortOrder.Ascending,
            LevelSortStrategy.UpdatedDesc => SortOrder.Descending,
            LevelSortStrategy.NameAsc => SortOrder.Ascending,
            LevelSortStrategy.NameDesc => SortOrder.Descending,
            LevelSortStrategy.CreatorAsc => SortOrder.Ascending,
            LevelSortStrategy.CreatorDesc => SortOrder.Descending,
            LevelSortStrategy.RecordCreatedAsc => SortOrder.Ascending,
            LevelSortStrategy.RecordCreatedDesc => SortOrder.Descending,
            LevelSortStrategy.RecordUpdatedAsc => SortOrder.Ascending,
            LevelSortStrategy.RecordUpdatedDesc => SortOrder.Descending,
            _ => throw new ArgumentOutOfRangeException(nameof(sortStrategy), sortStrategy, null),
        };
    }
}
