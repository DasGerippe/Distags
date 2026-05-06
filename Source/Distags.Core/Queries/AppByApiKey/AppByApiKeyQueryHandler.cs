using DataClasses;
using Distags.Contracts;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.AppByApiKey;

public class AppByApiKeyQueryHandler : IQueryHandler<AppByApiKeyQuery, AppDto?>
{
    private readonly DistagsDbContext _DbContext;

    public AppByApiKeyQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<AppDto?> HandleAsync(AppByApiKeyQuery query)
    {
        AppDto? appDto = await _DbContext.Set<App>()
            .Where(app => app.ApiKey == query.ApiKey)
            .Select(new AppMapper().Expression)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return appDto;
    }
}