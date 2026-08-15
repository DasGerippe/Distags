using System.Linq.Expressions;

namespace Distags.Core.Sorting;

public class PropertyQueryableSorter<TEntity> : IQueryableSorter<TEntity>
{
    private readonly Expression<Func<TEntity, object>> _PropertySelector;
    private readonly SortOrder _SortOrder;

    public PropertyQueryableSorter(Expression<Func<TEntity, object>> propertySelector, SortOrder sortOrder)
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
            _ => throw new InvalidOperationException($"Cannot process {nameof(SortOrder)} {_SortOrder}"),
        };
    }
}
