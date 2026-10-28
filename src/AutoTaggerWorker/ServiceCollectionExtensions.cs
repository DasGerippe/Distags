using AutoTagging;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTaggerWorker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoTaggerWorker(this IServiceCollection services)
    {
        services.AddAutoTagging();
        services.AddAutoTaggers();

        services.AddSingleton<AutoTaggerQueue>();
        services.AddHostedService<AutoTaggerScheduler>();
        services.AddHostedService<AutoTaggerBackgroundService>();

        return services;
    }
}
