namespace LevelTagApi.Conversion;

public interface IDtoConverterFactory<TEntity, TDto, TConversionParams>
    where TEntity : class
    where TDto : class
{
    IDtoConverter<TEntity, TDto> GetDtoConverter(TConversionParams conversionParams);
}