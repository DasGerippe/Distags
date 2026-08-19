using Distags.Core.Entities;

namespace Distags.Core.Sorting.Levels;

public class LevelSorterFactory : IQueryableSorterFactory<Level, LevelSortStrategy>
{
    public IQueryableSorter<Level> GetQueryableSorter(LevelSortStrategy sortProperty)
    {
        return sortProperty switch
        {
            LevelSortStrategy.CreatedAsc => new PropertyQueryableSorter<Level>(level => level.CreatedOn, SortOrder.Ascending),
            LevelSortStrategy.CreatedDesc => new PropertyQueryableSorter<Level>(level => level.CreatedOn, SortOrder.Descending),
            LevelSortStrategy.UpdatedAsc => new PropertyQueryableSorter<Level>(level => level.UpdatedOn, SortOrder.Ascending),
            LevelSortStrategy.UpdatedDesc => new PropertyQueryableSorter<Level>(level => level.UpdatedOn, SortOrder.Descending),
            LevelSortStrategy.NameAsc => new LevelNameSorter(SortOrder.Ascending),
            LevelSortStrategy.NameDesc => new LevelNameSorter(SortOrder.Descending),
            LevelSortStrategy.CreatorAsc => new LevelCreatorSorter(SortOrder.Ascending),
            LevelSortStrategy.CreatorDesc => new LevelCreatorSorter(SortOrder.Descending),
            _ => throw new ArgumentOutOfRangeException(nameof(sortProperty), sortProperty, null),
        };
    }
}
