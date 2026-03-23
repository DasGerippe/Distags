namespace LevelTagApi.Sorting;

public interface IQueryableSorterFactory<TEntity, TPropertySelectorEnum> where TPropertySelectorEnum : Enum
{
    IQueryableSorter<TEntity> GetQueryableSorter(TPropertySelectorEnum sortProperty);
}