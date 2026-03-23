using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Filtering;

public class StringFilter<TEntity> : IQueryableFilter<TEntity>
{
    private readonly Expression<Func<TEntity, bool>> _FilterExpression;

    public StringFilter(Expression<Func<TEntity, string>> propertySelector, string filterTerm)
    {
        if (!filterTerm.StartsWith('%') && !filterTerm.EndsWith('%'))
            filterTerm = $"%{filterTerm}%";

        _FilterExpression = CreateILikeExpression(propertySelector, filterTerm);
    }

    public IQueryable<TEntity> Filter(IQueryable<TEntity> source)
    {
        return source.Where(_FilterExpression);
    }

    private Expression<Func<TEntity, bool>> CreateILikeExpression(
        Expression<Func<TEntity, string>> propertySelector, string filterTerm)
    {
        // Manually build a lambda expression that's basically
        // entity => EF.Functions.ILike(propertySelector.Invoke(entity), filterTerm).
        // This way we can simplify the creation of filters based on specific entity properties
        // without having to specify the ILike function every time.

        MethodInfo iLikeMethod = typeof(NpgsqlDbFunctionsExtensions).GetMethod(
            nameof(NpgsqlDbFunctionsExtensions.ILike),
            [typeof(DbFunctions), typeof(string), typeof(string)])!;

        MethodCallExpression iLikeCall = Expression.Call(
            iLikeMethod,
            Expression.Constant(EF.Functions),
            propertySelector.Body,
            Expression.Constant(filterTerm));

        return Expression.Lambda<Func<TEntity, bool>>(iLikeCall, propertySelector.Parameters[0]);
    }
}