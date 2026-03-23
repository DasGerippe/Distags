using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LevelTagApi.Management;

public class AppManager : IAppManager
{
    private readonly IHttpContextAccessor _HttpContextAccessor;
    private readonly LevelTagDbContext _DbContext;

    public AppManager(IHttpContextAccessor httpContextAccessor, LevelTagDbContext dbContext)
    {
        _HttpContextAccessor = httpContextAccessor;
        _DbContext = dbContext;
    }

    public async Task<App?> GetCurrentAppAsync(params string[] includes)
    {
        Claim? appIdClaim = _HttpContextAccessor.HttpContext?.User.FindFirst(Constants.Claims.AppId);
        if (appIdClaim == null)
            return null;

        if (!long.TryParse(appIdClaim.Value, out long appId))
            return null;

        if (includes.Length == 0)
            return await _DbContext.FindAsync<App>(appId);

        IQueryable<App> apps = _DbContext.Set<App>()
            .Where(app => app.Id == appId);

        foreach (string include in includes)
        {
            apps = apps.Include(include);
        }

        return await apps.FirstOrDefaultAsync();
    }
}