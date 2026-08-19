using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.DisplayGroups;

public class DisplayGroupsQueryHandler : IQueryHandler<DisplayGroupsQuery, List<DisplayGroupDto>>
{
    private readonly DistagsDbContext _DbContext;

    public DisplayGroupsQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<DisplayGroupDto>> HandleAsync(DisplayGroupsQuery query)
    {
        return await _DbContext.Set<DisplayGroup>()
            .Select(new DisplayGroupMapper().Expression)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}
