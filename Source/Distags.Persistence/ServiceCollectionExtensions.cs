using Distags.Core.Filtering;
using Distags.Core.Interfaces;
using Distags.Persistence.Filtering;
using Distags.Persistence.Implementations;
using Distags.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Distags.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistagsPersistence(this IServiceCollection serviceCollection, string dbConnectionString)
    {
        serviceCollection.AddDbContext<Core.DistagsDbContext, DistagsDbContext>(ConfigureOptions(dbConnectionString));

        serviceCollection.AddTransient<ISequentialLevelActionExecutor, SequentialLevelActionExecutor>();

        serviceCollection.AddTransient<IStringContainsFilterFactory, StringContainsFilterFactory>();

        return serviceCollection;
    }

    private static Action<IServiceProvider, DbContextOptionsBuilder> ConfigureOptions(
        string connectionString,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction = null,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        return (services, options) =>
        {
            DataSeeder dataSeeder = new DataSeeder();

            options
                .UseNpgsql(connectionString, ConfigureNpgsqlOptions(npgsqlOptionsAction))
                .UseSnakeCaseNamingConvention()
                .UseSeeding(dataSeeder.Seed)
                .UseAsyncSeeding(dataSeeder.SeedAsync);

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