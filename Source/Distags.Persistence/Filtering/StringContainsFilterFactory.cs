using Distags.Core.Filtering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Distags.Persistence.Filtering;

public class StringContainsFilterFactory : IStringContainsFilterFactory
{
    public IQueryableFilter<TEntity> CreateFilter<TEntity>(Expression<Func<TEntity, string>> stringPropertySelector, string filterTerm)
    {
        if (!filterTerm.StartsWith('%') && !filterTerm.EndsWith('%'))
            filterTerm = $"%{filterTerm}%";

        // Build the lambda expression
        // entity => EF.Functions.ILike(entity.StringProperty, filterTerm).

        MethodInfo iLikeMethod = typeof(NpgsqlDbFunctionsExtensions).GetMethod(
            nameof(NpgsqlDbFunctionsExtensions.ILike),
            [typeof(DbFunctions), typeof(string), typeof(string)])!;

        MethodCallExpression iLikeCall = Expression.Call(
            iLikeMethod,
            Expression.Constant(EF.Functions),
            stringPropertySelector.Body,
            Expression.Constant(filterTerm));

        Expression<Func<TEntity, bool>> filterExpression =
            Expression.Lambda<Func<TEntity, bool>>(iLikeCall, stringPropertySelector.Parameters[0]);

        return new SimpleFilter<TEntity>(filterExpression);
    }
}
