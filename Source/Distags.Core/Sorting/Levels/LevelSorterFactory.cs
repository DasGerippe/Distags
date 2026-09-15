using Distags.Contracts;
using Distags.Core.Entities;

namespace Distags.Core.Sorting.Levels;

public class LevelSorterFactory : IQueryableSorterFactory<Level, LevelSortStrategy>
{
    public IQueryableSorter<Level> GetQueryableSorter(LevelSortStrategy sortProperty)
    {
        return sortProperty switch
        {
            LevelSortStrategy.CreatedAsc => new LevelCreatedOnSorter(SortOrder.Ascending),
            LevelSortStrategy.CreatedDesc => new LevelCreatedOnSorter(SortOrder.Descending),
            LevelSortStrategy.UpdatedAsc => new LevelUpdatedOnSorter(SortOrder.Ascending),
            LevelSortStrategy.UpdatedDesc => new LevelUpdatedOnSorter(SortOrder.Descending),
            LevelSortStrategy.NameAsc => new LevelNameSorter(SortOrder.Ascending),
            LevelSortStrategy.NameDesc => new LevelNameSorter(SortOrder.Descending),
            LevelSortStrategy.CreatorAsc => new LevelCreatorSorter(SortOrder.Ascending),
            LevelSortStrategy.CreatorDesc => new LevelCreatorSorter(SortOrder.Descending),
            _ => throw new ArgumentOutOfRangeException(nameof(sortProperty), sortProperty, null),
        };
    }
}
