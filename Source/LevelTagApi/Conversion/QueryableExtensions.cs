namespace LevelTagApi.Conversion;

public static class QueryableExtensions
{
    public static IQueryable<TDto> SelectDto<TEntity, TDto>(
        this IQueryable<TEntity> entities,
        IDtoConverter<TEntity, TDto> dtoConverter)
        where TEntity : class
        where TDto : class
    {
        return dtoConverter.SelectDto(entities);
    }

    public static IQueryable<TEntity> IncludeForDto<TEntity, TDto>(
        this IQueryable<TEntity> entities,
        IDtoConverter<TEntity, TDto> dtoConverter)
        where TEntity : class
        where TDto : class
    {
        return dtoConverter.IncludeRequiredNavigations(entities);
    }
}