using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Projection;

public class ContributionProjector : IEntityProjector<Contribution, ContributionDto>
{
    public Expression<Func<Contribution, ContributionDto>> Projection { get; }

    public ContributionProjector(bool ignoreAnonymity)
    {
        Projection = contribution => new ContributionDto
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