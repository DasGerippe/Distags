using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Mapping;
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
        ContributionMapper mapper = new ContributionMapper(query.IgnoreAnonymity);
        ContributionDto? contributionDto = await _DbContext.Set<Contribution>()
            .Where(contribution => contribution.Id == query.ContributionId)
            .Select(mapper.Expression)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        return contributionDto;
    }
}