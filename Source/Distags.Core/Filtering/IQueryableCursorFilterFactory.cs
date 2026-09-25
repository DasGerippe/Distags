using System.Runtime.CompilerServices;

namespace Distags.Core.Filtering;

public interface IQueryableCursorFilterFactory<TEntity, TPropertySelectorEnum> where TPropertySelectorEnum : Enum
{
    IQueryableFilter<TEntity> Create(TPropertySelectorEnum propertySelector, ITuple cursorTuple);
}
