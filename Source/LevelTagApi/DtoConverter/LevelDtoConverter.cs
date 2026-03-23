using DataClasses;
using LevelTagApi.Dtos;

namespace LevelTagApi.DtoConverter;

public class LevelDtoConverter : IDtoConverter<Level, GetLevelDto>
{
    public GetLevelDto ToDto(Level level)
    {
        return new GetLevelDto
        {
            Id = level.Id,
            Name = level.Name,
            CreatedOn = level.CreatedOn,
            UpdatedOn = level.UpdatedOn,
        };
    }

    public Level FromDto(GetLevelDto levelDto)
    {
        return new Level
        {
            Id = levelDto.Id,
            Name = levelDto.Name,
            CreatedOn = levelDto.CreatedOn,
            UpdatedOn = levelDto.UpdatedOn,
        };
    }
}