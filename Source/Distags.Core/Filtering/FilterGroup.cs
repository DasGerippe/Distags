namespace Distags.Core.Filtering;

public class FilterGroup<TEntity> : IQueryableFilter<TEntity>
{
    private readonly List<IQueryableFilter<TEntity>> _Filters = [];

    public void AddFilter(IQueryableFilter<TEntity> filter)
    {
        _Filters.Add(filter);
    }

    public void RemoveFilter(IQueryableFilter<TEntity> filter)
    {
        _Filters.Remove(filter);
    }

    public IQueryable<TEntity> Filter(IQueryable<TEntity> source)
    {
        foreach (IQueryableFilter<TEntity> filter in _Filters)
        {
            source = filter.Filter(source);
        }

        return source;
    }
}