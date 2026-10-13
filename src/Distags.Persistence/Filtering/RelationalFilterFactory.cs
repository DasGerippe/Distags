using Distags.Core.Filtering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Distags.Persistence.Filtering;

public class RelationalFilterFactory : IRelationalFilterFactory
{
    public IQueryableFilter<TEntity> CreateGreaterThanFilter<TEntity>(Expression<Func<TEntity, ITuple>> tupleSelector, ITuple tuple)
    {
        // entity => EF.Functions.GreaterThan(tupleSelector.Invoke(entity), tuple)
        return CreateFilter(nameof(NpgsqlDbFunctionsExtensions.GreaterThan), tupleSelector, tuple);
    }

    public IQueryableFilter<TEntity> CreateGreaterThanOrEqualFilter<TEntity>(Expression<Func<TEntity, ITuple>> tupleSelector, ITuple tuple)
    {
        // entity => EF.Functions.GreaterThanOrEqual(tupleSelector.Invoke(entity), tuple)
        return CreateFilter(nameof(NpgsqlDbFunctionsExtensions.GreaterThanOrEqual), tupleSelector, tuple);
    }

    public IQueryableFilter<TEntity> CreateLessThanFilter<TEntity>(Expression<Func<TEntity, ITuple>> tupleSelector, ITuple tuple)
    {
        // entity => EF.Functions.LessThan(tupleSelector.Invoke(entity), tuple)
        return CreateFilter(nameof(NpgsqlDbFunctionsExtensions.LessThan), tupleSelector, tuple);
    }

    public IQueryableFilter<TEntity> CreateLessThanOrEqualFilter<TEntity>(Expression<Func<TEntity, ITuple>> tupleSelector, ITuple tuple)
    {
        // entity => EF.Functions.LessThanOrEqual(tupleSelector.Invoke(entity), tuple)
        return CreateFilter(nameof(NpgsqlDbFunctionsExtensions.LessThanOrEqual), tupleSelector, tuple);
    }

    private static SimpleFilter<TEntity> CreateFilter<TEntity>(string methodName, Expression<Func<TEntity, ITuple>> tupleSelector, ITuple tuple)
    {
        MethodInfo method = typeof(NpgsqlDbFunctionsExtensions).GetMethod(methodName,
            [typeof(DbFunctions), typeof(ITuple), typeof(ITuple)])!;

        MethodCallExpression methodCall = Expression.Call(
            method,
            Expression.Constant(EF.Functions),
            tupleSelector.Body,
            Expression.Convert(Expression.Constant(tuple), typeof(ITuple)));

        Expression<Func<TEntity, bool>> filterExpression =
            Expression.Lambda<Func<TEntity, bool>>(methodCall, tupleSelector.Parameters[0]);

        return new SimpleFilter<TEntity>(filterExpression);
    }
}
