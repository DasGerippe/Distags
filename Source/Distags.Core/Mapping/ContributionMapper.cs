using DataClasses;
using Distags.Contracts;
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
            Level = new ContributionLevelDto
            {
                Id = contribution.Level.Id,
                Name = contribution.Level.Name,
                CreatorName = contribution.Level.Creator!.Name,
                CreatorAvatarHash = contribution.Level.Creator!.AvatarHash,
                ImageUrl = contribution.Level.IconUrl,
            },
            Changes = contribution.Changes
                .Select(change => new ContributionChangeDto
                {
                    Action = (ChangeActionDto)change.Action,
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