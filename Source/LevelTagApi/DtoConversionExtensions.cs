using DataClasses;
using LevelTagApi.Dtos;
using TagCompatibility = LevelTagApi.Dtos.TagCompatibility;

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

    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Name = tag.Name,
            DisplayName = tag.DisplayName,
        };
    }

    public static TagGroupDto ToDtoTree(this TagGroup tagGroup)
    {
        return new TagGroupDto
        {
            Name = tagGroup.Name,
            TagCompatibility = (TagCompatibility)tagGroup.Compatibility,
            Tags = tagGroup.Tags.OrderBy(tag => tag.Order).Select(ToDto).ToList(),
            SubGroups = tagGroup.SubGroups.OrderBy(tagGroup => tagGroup.Order).Select(ToDtoTree).ToList(),
        };
    }
}