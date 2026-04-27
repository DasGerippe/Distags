namespace Distags.Core.Filtering;

public interface IQueryableFilter<TEntity>
{
    IQueryable<TEntity> Filter(IQueryable<TEntity> source);
}