using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Distags.Core.Filtering;

public interface IRelationalFilterFactory
{
    IQueryableFilter<TEntity> CreateGreaterThanFilter<TEntity>(
        Expression<Func<TEntity, ITuple>> tupleSelector,
        ITuple tuple);

    IQueryableFilter<TEntity> CreateGreaterThanOrEqualFilter<TEntity>(
        Expression<Func<TEntity, ITuple>> tupleSelector,
        ITuple tuple);

    IQueryableFilter<TEntity> CreateLessThanFilter<TEntity>(
        Expression<Func<TEntity, ITuple>> tupleSelector,
        ITuple tuple);

    IQueryableFilter<TEntity> CreateLessThanOrEqualFilter<TEntity>(
        Expression<Func<TEntity, ITuple>> tupleSelector,
        ITuple tuple);
}
