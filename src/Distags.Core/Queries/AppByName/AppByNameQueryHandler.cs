using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.AppByName;

public class AppByNameQueryHandler : IQueryHandler<AppByNameQuery, AppDto>
{
    private readonly DistagsDbContext _DbContext;

    public AppByNameQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<AppDto> HandleAsync(AppByNameQuery query)
    {
        AppDto? appDto = await _DbContext.Set<App>()
            .Where(app => app.Name == query.AppName)
            .SelectDto()
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return appDto ?? throw new UnknownAppNameException(query.AppName);
    }
}
