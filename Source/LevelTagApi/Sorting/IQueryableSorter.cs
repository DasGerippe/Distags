namespace LevelTagApi.Sorting;

public interface IQueryableSorter<TEntity>
{
    public IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> source);
}