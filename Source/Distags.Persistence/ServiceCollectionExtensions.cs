using Distags.Core.Filtering;
using Distags.Core.Interfaces;
using Distags.Persistence.Filtering;
using Distags.Persistence.Implementations;
using Distags.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Distags.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistagsPersistence(
        this IServiceCollection serviceCollection,
        string dbConnectionString,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction = null,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        serviceCollection.AddDbContext<Core.DistagsDbContext, DistagsDbContext>(
            ConfigureOptions(dbConnectionString, optionsAction, npgsqlOptionsAction));

        serviceCollection.AddTransient<ISequentialLevelActionExecutor, SequentialLevelActionExecutor>();

        serviceCollection.AddTransient<IStringContainsFilterFactory, StringContainsFilterFactory>();

        serviceCollection.AddTransient<DefaultTagProvider>();
        serviceCollection.AddTransient<DataSeeder>();

        return serviceCollection;
    }

    private static Action<IServiceProvider, DbContextOptionsBuilder> ConfigureOptions(
        string connectionString,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction = null,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        return (services, options) =>
        {
            options
                .UseNpgsql(connectionString, ConfigureNpgsqlOptions(npgsqlOptionsAction))
                .UseSnakeCaseNamingConvention()
                .UseSeeding((dbContext, dataStoreChanged) =>
                {
                    DataSeeder dataSeeder = dbContext.GetService<DataSeeder>();
                    dataSeeder.Seed(dbContext, dataStoreChanged);
                })
                .UseAsyncSeeding(async (dbContext, dataStoreChanged, cancellationToken) =>
                {
                    DataSeeder dataSeeder = dbContext.GetService<DataSeeder>();
                    await dataSeeder.SeedAsync(dbContext, dataStoreChanged, cancellationToken).ConfigureAwait(false);
                });

            optionsAction?.Invoke(services, options);
        };
    }

    private static Action<NpgsqlDbContextOptionsBuilder> ConfigureNpgsqlOptions(
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        return npgsqlOptions =>
        {
            npgsqlOptionsAction?.Invoke(npgsqlOptions);
        };
    }
}
