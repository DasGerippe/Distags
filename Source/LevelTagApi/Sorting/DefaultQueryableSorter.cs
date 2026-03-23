using System.Linq.Expressions;

namespace LevelTagApi.Sorting;

public class DefaultQueryableSorter<TEntity> : IQueryableSorter<TEntity>
{
    private readonly Expression<Func<TEntity, object>> _PropertySelector;
    private readonly SortOrder _SortOrder;

    public DefaultQueryableSorter(Expression<Func<TEntity, object>> propertySelector, SortOrder sortOrder)
    {
        _PropertySelector = propertySelector;
        _SortOrder = sortOrder;
    }

    public virtual IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> source)
    {
        return _SortOrder switch
        {
            SortOrder.Ascending => source.OrderBy(_PropertySelector),
            SortOrder.Descending => source.OrderByDescending(_PropertySelector),
            _ => throw new ArgumentOutOfRangeException(nameof(_SortOrder), _SortOrder, null),
        };
    }
}