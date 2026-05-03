using System.Globalization;
using System.Security.Claims;

namespace LevelTagApi.Security;

public static class ClaimsPrincipalExtensions
{
    public static TValue? FindFirstValue<TValue>(this ClaimsPrincipal claimsPrincipal, string claimType)
        where TValue : struct, IParsable<TValue>
    {
        string? claimValue = claimsPrincipal.FindFirstValue(claimType);
        return claimValue == null ? null : TValue.Parse(claimValue, CultureInfo.InvariantCulture);
    }
}