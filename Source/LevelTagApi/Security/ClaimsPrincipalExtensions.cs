using System.Security.Claims;

namespace LevelTagApi.Security;

public static class ClaimsPrincipalExtensions
{
    public static long? GetSteamId(this ClaimsPrincipal claimsPrincipal)
    {
        Claim? steamIdClaim = claimsPrincipal.FindFirst(Constants.Claims.SteamId);
        return steamIdClaim != null
            ? long.Parse(steamIdClaim.Value)
            : null;
    }

    public static string? GetAppName(this ClaimsPrincipal claimsPrincipal)
    {
        Claim? appNameClaim = claimsPrincipal.FindFirst(Constants.Claims.AppName);
        return appNameClaim?.Value;
    }
}