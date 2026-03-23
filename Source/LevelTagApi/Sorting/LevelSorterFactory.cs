using DataClasses;
using LevelTagApi.Dtos;

namespace LevelTagApi.Sorting;

public class LevelSorterFactory : IQueryableSorterFactory<Level, LevelSortProperty>
{
    public IQueryableSorter<Level> GetQueryableSorter(LevelSortProperty sortProperty)
    {
        return sortProperty switch
        {
            LevelSortProperty.Created => new DefaultQueryableSorter<Level>(level => level.CreatedOn),
            LevelSortProperty.Updated => new DefaultQueryableSorter<Level>(level => level.UpdatedOn),
            LevelSortProperty.Name => new LevelNameSorter(),
            _ => throw new ArgumentOutOfRangeException(nameof(sortProperty), sortProperty, null),
        };
    }
}