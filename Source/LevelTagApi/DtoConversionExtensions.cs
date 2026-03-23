using DataClasses;
using LevelTagApi.Dtos;

namespace LevelTagApi;

public static class DtoConversionExtensions
{
    public static GetLevelDto ToDto(this Level level)
    {
        return new GetLevelDto
        {
            Id = level.Id,
            Name = level.Name,
            CreatorId = level.CreatorId,
            CreatorName = level.Creator?.Name,
            CreatorAvatarHash = level.Creator?.AvatarHash,
            FileName = level.FileName,
            FileUrl = level.FileUrl,
            IconUrl = level.IconUrl,
            CreatedOn = level.CreatedOn,
            UpdatedOn = level.UpdatedOn,
            Tags = level.Tags.Select(t => t.Name).ToList(),
        };
    }
}