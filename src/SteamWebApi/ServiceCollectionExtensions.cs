using AppHostUtils;
using Microsoft.Extensions.DependencyInjection;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.DelegatingHandlers;

namespace SteamWebApi;

public static class ServiceCollectionExtensions
{
    private const string PublicSteamApiUri = "https://api.steampowered.com/";

    public static IServiceCollection AddSteamWebApi(this IServiceCollection services, string configSectionPath)
    {
        services.AddValidatedConfigOptions<SteamWebApiSettings>(configSectionPath);
        services.AddValidatedConfigOptions<RetrySettings>($"{configSectionPath}:{nameof(RetrySettings)}");
        services.AddValidatedConfigOptions<RateLimitSettings>($"{configSectionPath}:{nameof(RateLimitSettings)}");

        services.AddTransient<RetryHandler>();
        services.AddTransient<RateLimitHandler>();

        services.AddHttpClient<SteamWebApiClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(PublicSteamApiUri);
            })
            .AddHttpMessageHandler<RetryHandler>()
            .AddHttpMessageHandler<RateLimitHandler>();

        services.AddTransient<IPublishedFileService, PublishedFileService>();
        services.AddTransient<ISteamUser, SteamUser>();

        return services;
    }
}
