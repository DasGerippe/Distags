using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Filtering;
using Distags.Core.Filtering.Contributions;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.Contributions;

public class ContributionsQueryHandler : IQueryHandler<ContributionsQuery, ContributionsPage>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IQueryableFilterFactory<Contribution, ContributionFilterParameters> _ContributionFilterFactory;

    public ContributionsQueryHandler(
        DistagsDbContext dbContext,
        IQueryableFilterFactory<Contribution, ContributionFilterParameters> contributionFilterFactory)
    {
        _DbContext = dbContext;
        _ContributionFilterFactory = contributionFilterFactory;
    }

    public async Task<ContributionsPage> HandleAsync(ContributionsQuery query)
    {
        IQueryable<Contribution> contributions = _DbContext.Set<Contribution>()
            .Where(contribution => !contribution.IsReverted && contribution.Changes.Any());

        ContributionFilterParameters filterParams = CreateFilterParameters(query);
        IQueryableFilter<Contribution> contributionFilter = _ContributionFilterFactory.GetQueryableFilter(filterParams);
        contributions = contributionFilter.Filter(contributions);

        contributions = query.DescendingOrder
            ? contributions.OrderByDescending(contribution => contribution.ContributedOn)
            : contributions.OrderBy(contribution => contribution.ContributedOn);

        contributions = contributions.Take(query.Limit);

        List<ContributionDto> contributionDtos = await (query.IgnoreAnonymity
            ? contributions.SelectDto()
            : contributions.SelectAnonymizedDto())
            .ToListAsync().ConfigureAwait(false);

        ContributionsPage contributionsPage = new ContributionsPage
        {
            Contributions = contributionDtos,
        };

        return contributionsPage;
    }

    private ContributionFilterParameters CreateFilterParameters(ContributionsQuery query)
    {
        return new ContributionFilterParameters
        {
            UserSteamId = query.UserSteamId,
            AppName = query.AppName,
            LevelSlug = query.LevelSlug,
            LevelWorkshopId = query.LevelWorkshopId,
            TagName = query.TagName,
            ContributedAfter = query.ContributedAfter,
            ContributedBefore = query.ContributedBefore,
            IgnoreAnonymity = query.IgnoreAnonymity,
        };
    }
}
