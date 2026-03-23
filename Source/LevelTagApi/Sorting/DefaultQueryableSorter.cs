using LevelTagApi.Dtos;
using System.Linq.Expressions;

namespace LevelTagApi.Sorting;

public class DefaultQueryableSorter<TEntity> : IQueryableSorter<TEntity>
{
    private readonly Expression<Func<TEntity, object>> _PropertySelector;

    public DefaultQueryableSorter(Expression<Func<TEntity, object>> propertySelector)
    {
        _PropertySelector = propertySelector;
    }

    public virtual IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> source, SortOrder sortOrder)
    {
        return sortOrder switch
        {
            SortOrder.Ascending => source.OrderBy(_PropertySelector),
            SortOrder.Descending => source.OrderByDescending(_PropertySelector),
            _ => throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null),
        };
    }
}