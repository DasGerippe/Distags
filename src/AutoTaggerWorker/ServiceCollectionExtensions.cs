using AutoTagging;
using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace AutoTaggerWorker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoTaggerWorker(this IServiceCollection services)
    {
        services.AddAutoTagging();

        RegisterAutoTaggerModuleServices(services);
        RegisterAutoTaggers(services);

        services.AddSingleton<AutoTaggerQueue>();
        services.AddHostedService<AutoTaggerScheduler>();
        services.AddHostedService<AutoTaggerBackgroundService>();

        return services;
    }

    private static void RegisterAutoTaggerModuleServices(IServiceCollection services)
    {
        IEnumerable<Assembly> assemblies = DependencyContext.Default!.GetDefaultAssemblyNames()
            .Where(assembly => assembly.FullName.StartsWith(nameof(AutoTagging)))
            .Select(Assembly.Load);

        List<IAutoTaggerModule> autoTaggerModules = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract && !type.IsInterface && type.IsAssignableTo(typeof(IAutoTaggerModule)))
            .Select(type => (IAutoTaggerModule)Activator.CreateInstance(type)!)
            .ToList();

        foreach (IAutoTaggerModule autoTaggerModule in autoTaggerModules)
        {
            autoTaggerModule.ConfigureServices(services);
        }
    }

    private static void RegisterAutoTaggers(IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies(assembly => assembly.FullName?.StartsWith(nameof(AutoTagging)) == true)
            .AddClasses(classes => classes.AssignableTo<AutoTagger>())
            .As<AutoTagger>()
            .AsSelf()
            .WithScopedLifetime());
    }
}
