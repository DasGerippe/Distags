using System.ComponentModel.DataAnnotations;

namespace SteamWebApi.DelegatingHandlers;

public class RateLimitSettings
{
    [Range(0, 60 * 1000)]
    public int RequestDelayMs { get; set; } = 1000;
}
