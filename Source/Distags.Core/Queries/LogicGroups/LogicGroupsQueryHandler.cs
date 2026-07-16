using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.LogicGroups;

public class LogicGroupsQueryHandler : IQueryHandler<LogicGroupsQuery, List<LogicGroupDto>>
{
    private readonly DistagsDbContext _DbContext;

    public LogicGroupsQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<LogicGroupDto>> HandleAsync(LogicGroupsQuery query)
    {
        return await _DbContext.Set<LogicGroup>()
            .Select(new LogicGroupMapper().Expression)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}