using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public interface IEntityMapper<TEntity, TDto>
{
    Expression<Func<TEntity, TDto>> Expression { get; }
}