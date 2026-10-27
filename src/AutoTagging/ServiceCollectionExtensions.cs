using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoTagging(this IServiceCollection services)
    {
        services.AddTransient<AutoTaggerRunner>();
        return services;
    }
}
