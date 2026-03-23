using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LevelTagApi;

public static class LevelTagDbContextExtensions
{
    public static async Task<User?> GetUser(
        this LevelTagDbContext dbContext,
        ClaimsPrincipal claimsPrincipal,
        params string[] includes)
    {
        Claim? steamIdClaim = claimsPrincipal.FindFirst(Constants.Claims.SteamId);
        if (steamIdClaim == null)
            return null;

        if (!long.TryParse(steamIdClaim.Value, out long steamId))
            return null;

        IQueryable<User> users = dbContext.Set<User>().Where(user => user.SteamId == steamId);
        foreach (string include in includes)
        {
            users = users.Include(include);
        }

        return await users.FirstOrDefaultAsync();
    }

    public static async Task<UserSession?> GetUserSession(
        this LevelTagDbContext dbContext,
        ClaimsPrincipal claimsPrincipal,
        params string[] includes)
    {
        Claim? sessionTokenClaim = claimsPrincipal.FindFirst(Constants.Claims.SessionToken);
        if (sessionTokenClaim == null)
            return null;

        IQueryable<UserSession> sessions = dbContext.Set<UserSession>().Where(session => session.Token == sessionTokenClaim.Value);
        foreach (string include in includes)
        {
            sessions = sessions.Include(include);
        }

        return await sessions.FirstOrDefaultAsync();
    }
}