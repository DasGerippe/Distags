using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.Apps;

public class AppsQueryHandler : IQueryHandler<AppsQuery, List<AppDto>>
{
    private readonly DistagsDbContext _DbContext;

    public AppsQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<AppDto>> HandleAsync(AppsQuery query)
    {
        return await _DbContext.Set<App>()
            .Select(new AppMapper().Expression)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}