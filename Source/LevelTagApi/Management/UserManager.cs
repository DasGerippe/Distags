using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LevelTagApi.Management;

public class UserManager : IUserManager
{
    private readonly IHttpContextAccessor _HttpContextAccessor;
    private readonly LevelTagDbContext _DbContext;

    public UserManager(IHttpContextAccessor httpContextAccessor, LevelTagDbContext dbContext)
    {
        _HttpContextAccessor = httpContextAccessor;
        _DbContext = dbContext;
    }

    public async Task<User?> GetCurrentUserAsync(params string[] includes)
    {
        Claim? steamIdClaim = _HttpContextAccessor.HttpContext?.User.FindFirst(Constants.Claims.SteamId);
        if (steamIdClaim == null)
            return null;

        if (!long.TryParse(steamIdClaim.Value, out long steamId))
            return null;

        IQueryable<User> users = _DbContext.Set<User>()
            .Where(user => user.SteamId == steamId);

        foreach (string include in includes)
        {
            users = users.Include(include);
        }

        return await users.FirstOrDefaultAsync();
    }

    public async Task<UserSession?> GetCurrentUserSessionAsync(params string[] includes)
    {
        Claim? sessionTokenClaim = _HttpContextAccessor.HttpContext?.User.FindFirst(Constants.Claims.SessionToken);
        if (sessionTokenClaim == null)
            return null;

        IQueryable<UserSession> sessions = _DbContext.Set<UserSession>()
            .Where(session => session.Token == sessionTokenClaim.Value);

        foreach (string include in includes)
        {
            sessions = sessions.Include(include);
        }

        return await sessions.FirstOrDefaultAsync();
    }
}