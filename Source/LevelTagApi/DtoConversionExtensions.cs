using DataClasses;
using LevelTagApi.Dtos.Apps;
using LevelTagApi.Dtos.Levels;
using LevelTagApi.Dtos.Tags;
using LevelTagApi.Dtos.UserBans;
using LevelTagApi.Dtos.Users;

namespace LevelTagApi;

public static class DtoConversionExtensions
{
    public static LevelDto ToDto(this Level level)
    {
        return new LevelDto
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

    public static TagGroupDto ToDto(this TagGroup tagGroup)
    {
        return new TagGroupDto
        {
            Name = tagGroup.Name,
            Tags = tagGroup.Tags
                .OrderBy(tag => tag.Position)
                .Select(tag => tag.ToDto())
                .ToList(),
        };
    }

    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Name = tag.Name,
            DisplayName = tag.DisplayName,
            Exclusivity = tag.Exclusivity,
            IsAutoTag = tag.IsAutoTag,
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            SteamId = user.SteamId ?? throw new InvalidOperationException("Cannot convert user without Steam ID to DTO."),
            SteamName = user.SteamName ?? string.Empty,
            SteamAvatarHash = user.SteamAvatarHash ?? string.Empty,
            AcceptsGuidelines = user.AcceptsGuidelines,
            IsAnonymous = user.IsAnonymous,
            IsBanned = user.IsBanned,
            Roles = user.Roles,
        };
    }

    public static UserDisplayDto ToDisplayDto(this User user)
    {
        return new UserDisplayDto
        {
            SteamId = user.SteamId,
            SteamName = user.SteamName,
            SteamAvatarHash = user.SteamAvatarHash,
        };
    }

    public static AppDto ToDto(this App app)
    {
        return new AppDto
        {
            Name = app.Name,
            ApiKey = app.ApiKey,
        };
    }
}