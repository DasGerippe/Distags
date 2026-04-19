using DataClasses;
using LevelTagApi.Dtos.Contributions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LevelTagApi.Conversion;

public sealed class ContributionDtoConverter : ExpressionBasedDtoConverter<Contribution, ContributionDto>
{
    private readonly ContributionConversionParameters _ConversionParameters;

    public ContributionDtoConverter(ContributionConversionParameters conversionParameters)
    {
        _ConversionParameters = conversionParameters;
    }

    public override IQueryable<Contribution> IncludeRequiredNavigations(IQueryable<Contribution> contributions)
    {
        if (_ConversionParameters.IncludeLevel)
            contributions = contributions
                .Include(contribution => contribution.Level.Creator);

        if (_ConversionParameters.IncludeContributor)
            contributions = contributions
                .Include(contribution => contribution.User)
                .Include(contribution => contribution.App);

        return contributions;
    }

    protected override Expression<Func<Contribution, ContributionDto>> CreateConversionExpression()
    {
        bool ignoreAnonymity = _ConversionParameters.IgnoreAnonymity;
        bool includeLevel = _ConversionParameters.IncludeLevel;
        bool includeContributor = _ConversionParameters.IncludeContributor;

        return contribution => new ContributionDto
        {
            Id = contribution.Id,
            Level = !includeLevel ? null : new LevelDto
            {
                Id = contribution.Level.Id,
                Name = contribution.Level.Name,
                CreatorName = contribution.Level.Creator!.Name,
                CreatorAvatarHash = contribution.Level.Creator!.AvatarHash,
                ImageUrl = contribution.Level.IconUrl,
            },
            Changes = contribution.Changes
                .Select(change => new ChangeDto
                {
                    Action = (TagAction)change.Action,
                    TagName = change.TagName,
                })
                .ToList(),
            User = !includeContributor || contribution.User == null ? null : new UserDto
            {
                IsAnonymous = contribution.User!.IsAnonymous,
                SteamId = ignoreAnonymity || !contribution.User!.IsAnonymous ? contribution.User!.SteamId : null,
                SteamName = ignoreAnonymity || !contribution.User!.IsAnonymous ? contribution.User!.SteamName : null,
                SteamAvatarHash = ignoreAnonymity || !contribution.User!.IsAnonymous ? contribution.User!.SteamAvatarHash : null,
                IsBanned = contribution.User!.IsBanned,
            },
            App = !includeContributor || contribution.App == null ? null : new AppDto
            {
                Name = contribution.App!.Name,
            },
            ContributedOn = contribution.ContributedOn,
        };
    }
}