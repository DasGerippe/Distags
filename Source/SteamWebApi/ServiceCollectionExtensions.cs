using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.DelegatingHandlers;

namespace SteamWebApi;

public static class ServiceCollectionExtensions
{
    private const string PublicSteamApiUri = "https://api.steampowered.com/";

    public static IServiceCollection AddSteamWebApi(this IServiceCollection services, string configSectionPath)
    {
        services.AddOptions<SteamWebApiSettings>()
            .BindConfiguration(configSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddKeyedSingleton<string>("SteamWebApiKey", (serviceProvider, key) =>
            serviceProvider.GetRequiredService<IOptions<SteamWebApiSettings>>().Value.ApiKey);

        services.AddTransient<RetrySettings>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<SteamWebApiSettings>>().Value.RetrySettings);

        services.AddTransient<RateLimitSettings>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<SteamWebApiSettings>>().Value.RateLimitSettings);

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