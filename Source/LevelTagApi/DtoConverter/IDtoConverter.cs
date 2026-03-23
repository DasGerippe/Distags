namespace LevelTagApi.DtoConverter;

public interface IDtoConverter<TEntity, TDto>
{
    TDto ToDto(TEntity entity);

    TEntity FromDto(TDto dto);
}