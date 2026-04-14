using System.Linq.Expressions;

namespace LevelTagApi.Conversion;

public abstract class ExpressionBasedDtoConverter<TEntity, TDto> : IDtoConverter<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    private readonly Lazy<Expression<Func<TEntity, TDto>>> _ConversionExpression;
    private readonly Lazy<Func<TEntity, TDto>> _CompiledConversionExpression;

    protected ExpressionBasedDtoConverter()
    {
        _ConversionExpression = new Lazy<Expression<Func<TEntity, TDto>>>(CreateConversionExpression);
        _CompiledConversionExpression = new Lazy<Func<TEntity, TDto>>(() => _ConversionExpression.Value.Compile());
    }

    public abstract IQueryable<TEntity> IncludeRequiredNavigations(IQueryable<TEntity> entities);

    public IQueryable<TDto> SelectDto(IQueryable<TEntity> entities)
    {
        return entities
            .IncludeForDto(this)
            .Select(_ConversionExpression.Value);
    }

    public TDto ToDto(TEntity entity)
    {
        return _CompiledConversionExpression.Value.Invoke(entity);
    }

    protected abstract Expression<Func<TEntity, TDto>> CreateConversionExpression();
}