using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.Apps;

public class AppsQueryHandler : IQueryHandler<AppsQuery, List<AppDto>>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IEntityProjector<App, AppDto> _AppProjector;

    public AppsQueryHandler(DistagsDbContext dbContext, IEntityProjector<App, AppDto> appProjector)
    {
        _DbContext = dbContext;
        _AppProjector = appProjector;
    }

    public async Task<List<AppDto>> HandleAsync(AppsQuery query)
    {
        return await _DbContext.Set<App>()
            .Select(_AppProjector.Projection)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}