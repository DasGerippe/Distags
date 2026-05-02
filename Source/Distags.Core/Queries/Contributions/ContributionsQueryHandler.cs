using DataClasses;
using Distags.Core.Dtos;
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

        ContributionsCursor? cursor = CursorEncoder.Decode<ContributionsCursor>(query.Cursor);
        if (cursor != null)
            contributions = contributions.Where(contribution => contribution.ContributedOn < cursor.ContributedOn);

        ContributionFilterParameters filterParams = CreateFilterParameters(query);
        IQueryableFilter<Contribution> contributionFilter = _ContributionFilterFactory.GetQueryableFilter(filterParams);
        contributions = contributionFilter.Filter(contributions);

        contributions = contributions
            .OrderByDescending(contribution => contribution.ContributedOn)
            .Take(query.Limit);

        ContributionProjector contributionProjector = new ContributionProjector(query.IgnoreAnonymity);
        List<ContributionDto> contributionDtos = await contributions
            .Select(contributionProjector.Projection)
            .ToListAsync()
            .ConfigureAwait(false);

        ContributionsPage contributionsPage = new ContributionsPage
        {
            Contributions = contributionDtos,
            NextCursor = GetNextCursor(contributionDtos, query.Limit),
        };

        return contributionsPage;
    }

    private ContributionFilterParameters CreateFilterParameters(ContributionsQuery query)
    {
        return new ContributionFilterParameters
        {
            UserSteamId = query.UserSteamId,
            AppName = query.AppName,
            LevelId = query.LevelId,
            TagName = query.TagName,
            IgnoreAnonymity = query.IgnoreAnonymity,
        };
    }

    private string? GetNextCursor(List<ContributionDto> contributionDtos, int queryLimit)
    {
        if (contributionDtos.Count == 0 || contributionDtos.Count < queryLimit)
            return null;

        ContributionDto lastContribution = contributionDtos[^1];
        ContributionsCursor nextCursor = new ContributionsCursor
        {
            ContributedOn = lastContribution.ContributedOn,
        };

        return CursorEncoder.Encode(nextCursor);
    }
}