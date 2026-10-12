namespace Distags.Core.Sorting;

public interface IQueryableSorterFactory<TEntity, TPropertySelectorEnum> where TPropertySelectorEnum : Enum
{
    IQueryableSorter<TEntity> GetQueryableSorter(TPropertySelectorEnum sortProperty);
}
