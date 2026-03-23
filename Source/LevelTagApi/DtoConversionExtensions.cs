using DataClasses;
using LevelTagApi.Dtos;

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
                .OrderBy(tag => tag.Order)
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
            SteamName = user.SteamName,
            SteamAvatarHash = user.SteamAvatarHash,
            IsAnonymous = user.IsAnonymous,
            Roles = user.Roles,
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

    public static ContributionDto ToDto(this Contribution contribution, bool respectAnonymity)
    {
        return new ContributionDto
        {
            Level = contribution.Level.ToDto(),
            Tag = contribution.Tag.ToDto(),
            Contributor = GetContributorDto(contribution, respectAnonymity),
            Action = (ContributionAction)contribution.Action,
            ContributedOn = contribution.ContributedOn,
        };
    }

    private static ContributorDto GetContributorDto(Contribution contribution, bool respectAnonymity)
    {
        if (contribution.User != null)
        {
            return new ContributorUserDto
            {
                IsAnonymous = contribution.User.IsAnonymous,
                SteamId = respectAnonymity && contribution.User.IsAnonymous ? null : contribution.User.SteamId,
                SteamName = respectAnonymity && contribution.User.IsAnonymous ? null : contribution.User.SteamName,
                SteamAvatarHash = respectAnonymity && contribution.User.IsAnonymous ? null : contribution.User.SteamAvatarHash,
            };
        }
        else if (contribution.App != null)
        {
            return new ContributorAppDto
            {
                Name = contribution.App.Name,
            };
        }
        else
        {
            throw new ArgumentException(
                $"{nameof(Contribution)} has neither {nameof(Contribution.User)} nor {nameof(Contribution.App)}.",
                nameof(contribution));
        }
    }
}