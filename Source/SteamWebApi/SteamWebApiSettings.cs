using SteamWebApi.DelegatingHandlers;

namespace SteamWebApi;

public class SteamWebApiSettings
{
    public string ApiKey { get; set; } = string.Empty;

    public RetrySettings RetrySettings { get; set; } = new RetrySettings();

    public RateLimitSettings RateLimitSettings { get; set; } = new RateLimitSettings();
}