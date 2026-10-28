using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoTagging(this IServiceCollection services)
    {
        services.AddTransient<AutoTaggerRunner>();
        return services;
    }

    public static IServiceCollection AddAutoTaggers(this IServiceCollection services, params string[] autoTaggerNames)
    {
        RegisterAutoTaggers(services, autoTaggerNames);
        RegisterAutoTaggerModuleServices(services);
        return services;
    }

    private static void RegisterAutoTaggers(IServiceCollection services, params string[] autoTaggerNames)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies(assembly => assembly.FullName?.StartsWith(nameof(AutoTagging)) == true)
            .AddClasses(classes => classes
                .AssignableTo<AutoTagger>()
                .Where(type => autoTaggerNames.Length == 0 || autoTaggerNames.Contains(type.Name)))
            .As<AutoTagger>()
            .AsSelf()
            .WithScopedLifetime());
    }

    private static void RegisterAutoTaggerModuleServices(IServiceCollection services)
    {
        IEnumerable<Type> registeredAutoTaggerTypes = services
            .Where(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(AutoTagger) &&
                serviceDescriptor.ImplementationType != null &&
                serviceDescriptor.ImplementationType != typeof(AutoTagger))
            .Select(serviceDescriptor => serviceDescriptor.ImplementationType!);

        List<IAutoTaggerModule> autoTaggerModules = registeredAutoTaggerTypes
            .SelectMany(autoTaggerType => autoTaggerType.Assembly.GetTypes())
            .Where(type => !type.IsAbstract && !type.IsInterface && type.IsAssignableTo(typeof(IAutoTaggerModule)))
            .Select(type => (IAutoTaggerModule)Activator.CreateInstance(type)!)
            .ToList();

        foreach (IAutoTaggerModule autoTaggerModule in autoTaggerModules)
        {
            autoTaggerModule.ConfigureServices(services);
        }
    }
}
