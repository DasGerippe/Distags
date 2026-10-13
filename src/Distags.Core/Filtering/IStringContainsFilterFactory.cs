using System.Linq.Expressions;

namespace Distags.Core.Filtering;

public interface IStringContainsFilterFactory
{
    IQueryableFilter<TEntity> CreateFilter<TEntity>(Expression<Func<TEntity, string>> stringPropertySelector, string filterTerm);
}
