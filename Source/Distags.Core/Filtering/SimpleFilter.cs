using System.Linq.Expressions;

namespace Distags.Core.Filtering;

public class SimpleFilter<TEntity> : IQueryableFilter<TEntity>
{
    private readonly Expression<Func<TEntity, bool>> _FilterExpression;

    public SimpleFilter(Expression<Func<TEntity, bool>> filterExpression)
    {
        _FilterExpression = filterExpression;
    }

    public IQueryable<TEntity> Filter(IQueryable<TEntity> source)
    {
        return source.Where(_FilterExpression);
    }
}
