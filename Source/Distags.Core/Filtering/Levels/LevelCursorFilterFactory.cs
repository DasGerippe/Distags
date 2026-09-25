using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Expressions;
using Distags.Core.Sorting;
using Distags.Core.Sorting.Levels;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Distags.Core.Filtering.Levels;

public class LevelCursorFilterFactory : IQueryableCursorFilterFactory<Level, LevelSortStrategy>
{
    private readonly IRelationalFilterFactory _RelationalFilterFactory;

    public LevelCursorFilterFactory(IRelationalFilterFactory relationalFilterFactory)
    {
        _RelationalFilterFactory = relationalFilterFactory;
    }

    public IQueryableFilter<Level> Create(LevelSortStrategy sortStrategy, ITuple cursorTuple)
    {
        SortOrder sortOrder = sortStrategy.GetSortOrder();
        Expression<Func<Level, ITuple>> cursorTupleSelector = GetCursorTupleSelector(sortStrategy);

        return sortOrder switch
        {
            SortOrder.Ascending => _RelationalFilterFactory.CreateGreaterThanFilter(cursorTupleSelector, cursorTuple),
            SortOrder.Descending => _RelationalFilterFactory.CreateLessThanFilter(cursorTupleSelector, cursorTuple),
            _ => throw new ArgumentOutOfRangeException(
                nameof(sortStrategy), sortStrategy, $"Argument has an unsupported sort order: {sortOrder}."),
        };
    }

    private static Expression<Func<Level, ITuple>> GetCursorTupleSelector(LevelSortStrategy sortStrategy)
    {
        return sortStrategy switch
        {
            LevelSortStrategy.CreatedAsc or LevelSortStrategy.CreatedDesc => AsCursorWithId(LevelExpressions.CreatedOnSelector),
            LevelSortStrategy.UpdatedAsc or LevelSortStrategy.UpdatedDesc => AsCursorWithId(LevelExpressions.UpdatedOnSelector),
            LevelSortStrategy.NameAsc or LevelSortStrategy.NameDesc => AsCursorWithId(level => level.Name),
            LevelSortStrategy.CreatorAsc or LevelSortStrategy.CreatorDesc => AsCursorWithId(LevelExpressions.CreatorNameSelector),
            LevelSortStrategy.RecordCreatedAsc or LevelSortStrategy.RecordCreatedDesc => AsCursorWithId(level => level.RecordCreatedOn),
            LevelSortStrategy.RecordUpdatedAsc or LevelSortStrategy.RecordUpdatedDesc => AsCursorWithId(level => level.RecordUpdatedOn),
            _ => throw new ArgumentOutOfRangeException(nameof(sortStrategy), sortStrategy, null),
        };
    }

    private static Expression<Func<Level, ITuple>> AsCursorWithId<TProperty>(Expression<Func<Level, TProperty>> propertySelector)
    {
        ParameterExpression levelParameter = propertySelector.Parameters[0];
        MemberExpression levelIdExpression = Expression.Property(levelParameter, nameof(Level.Id));

        MethodInfo valueTupleCreateMethod = typeof(ValueTuple)
            .GetMethods()
            .Single(method => method.Name == nameof(ValueTuple.Create) && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TProperty), levelIdExpression.Type);

        MethodCallExpression valueTupleCreateCall = Expression.Call(valueTupleCreateMethod, propertySelector.Body, levelIdExpression);
        UnaryExpression convertTupleExpression = Expression.Convert(valueTupleCreateCall, typeof(ITuple));
        return Expression.Lambda<Func<Level, ITuple>>(convertTupleExpression, levelParameter);
    }
}
