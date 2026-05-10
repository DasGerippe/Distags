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

        serviceCollection.AddTransient<IRequestDispatcher, RequestDispatcher>();
        serviceCollection.Scan(scan => scan
            .FromAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces());

        serviceCollection.AddSingleton<AppEventQueue>();
        serviceCollection.AddHostedService<AppEventDispatcherService>();

        return serviceCollection;
    }
}