using Distags.Core.Commands;
using Distags.Core.Entities;
using Distags.Core.Events;
using Distags.Core.Filtering;
using Distags.Core.Filtering.Contributions;
using Distags.Core.Filtering.Levels;
using Distags.Core.Management;
using Distags.Core.Management.History;
using Distags.Core.Queries;
using Distags.Core.Security;
using Distags.Core.Services;
using Distags.Core.Sorting;
using Distags.Core.Sorting.Levels;
using Microsoft.Extensions.DependencyInjection;

namespace Distags.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistagsCore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<TokenGenerator>();

        serviceCollection.AddTransient<ContributionManager>();
        serviceCollection.AddTransient<TagChangeManager>();
        serviceCollection.AddTransient<ContributionReplayEngine>();
        serviceCollection.AddTransient<UserBanPermissionChecker>();

        serviceCollection.AddTransient<MaintenanceManager>();

        serviceCollection.AddTransient<IQueryableFilterFactory<Contribution, ContributionFilterParameters>, ContributionFilterFactory>();
        serviceCollection.AddTransient<IQueryableFilterFactory<Level, LevelFilterParameters>, LevelFilterFactory>();

        serviceCollection.AddTransient<IQueryableSorterFactory<Level, LevelSortStrategy>, LevelSorterFactory>();

        serviceCollection.AddGenericServiceImplementations(typeof(IQueryHandler<,>));
        serviceCollection.AddGenericServiceImplementations(typeof(ICommandHandler<>));

        serviceCollection.AddSingleton<AppEventQueue>();
        serviceCollection.AddHostedService<AppEventDispatcherService>();

        return serviceCollection;
    }

    private static IServiceCollection AddGenericServiceImplementations(
        this IServiceCollection serviceCollection, Type genericServiceType)
    {
        IEnumerable<Type> classTypes = typeof(ServiceCollectionExtensions).Assembly.GetTypes()
            .Where(type => !type.IsInterface && !type.IsAbstract);

        foreach (Type classType in classTypes)
        {
            IEnumerable<Type> queryHandlerInterfaces = classType.GetInterfaces()
                .Where(classInterface =>
                    classInterface.IsGenericType &&
                    classInterface.GetGenericTypeDefinition() == genericServiceType);

            foreach (Type queryHandlerInterface in queryHandlerInterfaces)
            {
                serviceCollection.Add(new ServiceDescriptor(queryHandlerInterface, classType, ServiceLifetime.Transient));
            }
        }

        return serviceCollection;
    }
}