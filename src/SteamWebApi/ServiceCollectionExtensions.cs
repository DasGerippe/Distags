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
        AddValidatedConfigOptions<SteamWebApiSettings>(services, configSectionPath);
        AddValidatedConfigOptions<RetrySettings>(services, $"{configSectionPath}:{nameof(RetrySettings)}");
        AddValidatedConfigOptions<RateLimitSettings>(services, $"{configSectionPath}:{nameof(RateLimitSettings)}");

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

    private static void AddValidatedConfigOptions<TOptions>(IServiceCollection services, string configSectionPath)
        where TOptions : class
    {
        services
            .AddOptions<TOptions>()
            .BindConfiguration(configSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<TOptions>(services => services.GetRequiredService<IOptions<TOptions>>().Value);
    }
}
