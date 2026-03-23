using System.Security.Claims;
using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi;

public static class LevelTagDbContextExtensions
{
    public static async Task<User?> GetSessionUser(this LevelTagDbContext dbContext, ClaimsPrincipal claimsPrincipal)
    {
        Claim? steamIdClaim = claimsPrincipal.FindFirst(Constants.Claims.SteamId);
        if (steamIdClaim == null)
            return null;

        if (!long.TryParse(steamIdClaim.Value, out long steamId))
            return null;

        return await dbContext.Set<User>().FirstOrDefaultAsync(user => user.SteamId == steamId);
    }
}