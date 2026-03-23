using DataClasses;
using LevelTagApi.Dtos;
using Microsoft.EntityFrameworkCore;
using ContributionAction = LevelTagApi.Dtos.ContributionAction;

namespace LevelTagApi.Conversion;

public class ContributionDtoConverter
{
    public IQueryable<Contribution> IncludeRequiredNavigations(IQueryable<Contribution> contributions)
    {
        return contributions
            .Include(contribution => contribution.Level.Creator)
            .Include(contribution => contribution.Tag)
            .Include(contribution => contribution.User)
            .Include(contribution => contribution.App);
    }

    public IQueryable<ContributionDto> SelectDto(IQueryable<Contribution> contributions, bool respectAnonymity)
    {
        return IncludeRequiredNavigations(contributions)
            .Select(contribution => new ContributionDto
            {
                Id = contribution.Id,
                Level = contribution.Level.ToDto(),
                Tag = contribution.Tag.ToDto(),
                Contributor = GetContributorDto(contribution, respectAnonymity),
                Action = (ContributionAction)contribution.Action,
                ContributedOn = contribution.ContributedOn,
            });
    }

    public ContributionDto ToDto(Contribution contribution, bool respectAnonymity)
    {
        return new ContributionDto
        {
            Id = contribution.Id,
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