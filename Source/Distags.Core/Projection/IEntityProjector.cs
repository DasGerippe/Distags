using System.Linq.Expressions;

namespace Distags.Core.Projection;

public interface IEntityProjector<TEntity, TDto>
{
    Expression<Func<TEntity, TDto>> Projection { get; }
}