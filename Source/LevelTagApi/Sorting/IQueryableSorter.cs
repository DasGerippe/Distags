namespace LevelTagApi.Sorting;

public interface IQueryableSorter<T>
{
    public IOrderedQueryable<T> Sort(IQueryable<T> source);
}