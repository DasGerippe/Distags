using System.Linq.Expressions;
using Distags.Core.Filtering;

namespace Distags.Infrastructure.Filtering;

public class StringContainsFilterFactory : IStringContainsFilterFactory
{
    public IQueryableFilter<TEntity> CreateFilter<TEntity>(Expression<Func<TEntity, string>> stringPropertySelector, string filterTerm)
    {
        return new StringContainsFilter<TEntity>(stringPropertySelector, filterTerm);
    }
}