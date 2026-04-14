namespace LevelTagApi.Conversion;

public interface IDtoConverter<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    IQueryable<TEntity> IncludeRequiredNavigations(IQueryable<TEntity> entities);

    IQueryable<TDto> SelectDto(IQueryable<TEntity> entities);

    TDto ToDto(TEntity entity);
}