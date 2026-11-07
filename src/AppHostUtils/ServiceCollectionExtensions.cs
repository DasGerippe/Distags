using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppHostUtils;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidatedConfigOptions<TOptions>(this IServiceCollection services, string configSectionPath)
        where TOptions : class
    {
        services
            .AddOptions<TOptions>()
            .BindConfiguration(configSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<TOptions>(serviceProvider => serviceProvider.GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }
}
