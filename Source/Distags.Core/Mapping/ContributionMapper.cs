using Distags.Contracts;
using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class ContributionMapper : IEntityMapper<Contribution, ContributionDto>
{
    public Expression<Func<Contribution, ContributionDto>> Expression { get; }

    public ContributionMapper(bool ignoreAnonymity)
    {
        Expression = contribution => new ContributionDto
        {
            Id = contribution.Id,
            Level = contribution.Level.Type == LevelType.Official
                ? new OfficialLevelDto
                {
                    Slug = (contribution.Level as OfficialLevel)!.Slug,
                    Name = contribution.Level.Name,
                    CreatorName = (contribution.Level as OfficialLevel)!.CreatorName,
                    GameModes = (contribution.Level as OfficialLevel)!.GameModes,
                    Tags = contribution.Level.Tags.Select(tag => tag.Name).ToList(),
                }
                : new WorkshopLevelDto
                {
                    WorkshopId = (contribution.Level as WorkshopLevel)!.WorkshopId,
                    Name = contribution.Level.Name,
                    CreatorId = (contribution.Level as WorkshopLevel)!.CreatorId,
                    CreatorName = (contribution.Level as WorkshopLevel)!.Creator!.Name,
                    CreatorAvatarHash = (contribution.Level as WorkshopLevel)!.Creator!.AvatarHash,
                    FileName = (contribution.Level as WorkshopLevel)!.FileName,
                    FileUrl = (contribution.Level as WorkshopLevel)!.FileUrl,
                    ImageUrl = (contribution.Level as WorkshopLevel)!.ImageUrl,
                    CreatedOn = (contribution.Level as WorkshopLevel)!.CreatedOn,
                    UpdatedOn = (contribution.Level as WorkshopLevel)!.UpdatedOn,
                    WorkshopTags = (contribution.Level as WorkshopLevel)!.WorkshopTags,
                    Tags = contribution.Level.Tags.Select(tag => tag.Name).ToList(),
                },
            Changes = contribution.Changes
                .Select(change => new TagChangeDto
                {
                    Action = change.Action,
                    TagName = change.TagName,
                })
                .ToList(),
            User = contribution.User == null ? null : new ContributionUserDto
            {
                IsAnonymous = contribution.User!.IsAnonymous,
                SteamId = ignoreAnonymity || !contribution.User.IsAnonymous ? contribution.User!.SteamId : null,
                SteamName = ignoreAnonymity || !contribution.User.IsAnonymous ? contribution.User!.SteamName : null,
                SteamAvatarHash = ignoreAnonymity || !contribution.User.IsAnonymous ? contribution.User!.SteamAvatarHash : null,
                IsBanned = contribution.User!.IsBanned,
            },
            App = contribution.App == null ? null : new ContributionAppDto
            {
                Name = contribution.App!.Name,
            },
            ContributedOn = contribution.ContributedOn,
        };
    }
}
