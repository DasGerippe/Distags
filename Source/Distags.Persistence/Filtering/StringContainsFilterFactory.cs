using Distags.Core.Filtering;
using System.Linq.Expressions;

namespace Distags.Persistence.Filtering;

public class StringContainsFilterFactory : IStringContainsFilterFactory
{
    public IQueryableFilter<TEntity> CreateFilter<TEntity>(Expression<Func<TEntity, string>> stringPropertySelector, string filterTerm)
    {
        return new StringContainsFilter<TEntity>(stringPropertySelector, filterTerm);
    }
}
