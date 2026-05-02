using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Exceptions;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.AppByName;

public class AppByNameQueryHandler : IQueryHandler<AppByNameQuery, AppDto>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IEntityProjector<App, AppDto> _AppProjector;

    public AppByNameQueryHandler(DistagsDbContext dbContext, IEntityProjector<App, AppDto> appProjector)
    {
        _DbContext = dbContext;
        _AppProjector = appProjector;
    }

    public async Task<AppDto> HandleAsync(AppByNameQuery query)
    {
        AppDto? appDto = await _DbContext.Set<App>()
            .Where(app => app.Name == query.AppName)
            .Select(_AppProjector.Projection)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return appDto ?? throw new UnknownAppNameException(query.AppName);
    }
}