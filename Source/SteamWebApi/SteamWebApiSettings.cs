using SteamWebApi.DelegatingHandlers;

namespace SteamWebApi;

public class SteamWebApiSettings
{
    public RetrySettings RetrySettings { get; set; } = new RetrySettings();

    public RateLimitSettings RateLimitSettings { get; set; } = new RateLimitSettings();
}