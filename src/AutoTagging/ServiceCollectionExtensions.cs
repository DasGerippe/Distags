using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoTagging(this IServiceCollection services)
    {
        services.AddTransient<AutoTaggerSeeder>();
        services.AddSingleton<AutoTaggerHost>();
        return services;
    }

    public static IServiceCollection AddAutoTaggers(this IServiceCollection services, params string[] autoTaggerNames)
    {
        return services.Scan(scan => scan
            .FromApplicationDependencies(assembly => assembly.FullName?.StartsWith(nameof(AutoTagging)) == true)
            .AddClasses(classes => classes
                .AssignableTo<IAutoTagger>()
                .Where(type => autoTaggerNames.Length == 0 || autoTaggerNames.Contains(type.Name)))
            .AsSelfWithInterfaces(interfaceType => interfaceType == typeof(IAutoTagger))
            .WithScopedLifetime());
    }
}
