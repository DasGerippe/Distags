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
using Microsoft.Extensions.Options;

namespace Distags.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistagsCore(this IServiceCollection services, string? configSectionPath = null)
    {
        configSectionPath ??= $"{nameof(Distags)}:{nameof(Core)}";
        services.AddValidatedConfigOptions<DistagsCoreSettings>(configSectionPath);

        services.AddTransient<TokenGenerator>();

        services.AddTransient<ContributionManager>();
        services.AddTransient<TagChangeManager>();
        services.AddTransient<ContributionReplayEngine>();
        services.AddTransient<UserBanPermissionChecker>();

        services.AddTransient<MaintenanceManager>();

        services.AddTransient<IQueryableFilterFactory<Contribution, ContributionFilterParameters>, ContributionFilterFactory>();
        services.AddTransient<IQueryableFilterFactory<Level, LevelFilterParameters>, LevelFilterFactory>();

        services.AddTransient<IQueryableSorterFactory<Level, LevelSortStrategy>, LevelSorterFactory>();

        services.AddTransient<IRequestDispatcher, RequestDispatcher>();
        services.Scan(scan => scan
            .FromAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces());

        services.AddSingleton<AppEventQueue>();
        services.AddHostedService<AppEventDispatcherService>();

        return services;
    }

    private static IServiceCollection AddValidatedConfigOptions<TOptions>(this IServiceCollection services, string configSectionPath)
        where TOptions : class
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(configSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<TOptions>(services => services.GetRequiredService<IOptions<TOptions>>().Value);
        return services;
    }
}