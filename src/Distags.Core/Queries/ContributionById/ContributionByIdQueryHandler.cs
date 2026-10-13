using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.ContributionById;

public class ContributionByIdQueryHandler : IQueryHandler<ContributionByIdQuery, ContributionDto?>
{
    private readonly DistagsDbContext _DbContext;

    public ContributionByIdQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<ContributionDto?> HandleAsync(ContributionByIdQuery query)
    {
        IQueryable<Contribution> contributions = _DbContext.Set<Contribution>()
            .Where(contribution => contribution.Id == query.ContributionId);

        IQueryable<ContributionDto> contributionDtos = query.IgnoreAnonymity
            ? contributions.SelectDto()
            : contributions.SelectAnonymizedDto();

        ContributionDto? contributionDto = await contributionDtos.FirstOrDefaultAsync().ConfigureAwait(false);
        return contributionDto;
    }
}
