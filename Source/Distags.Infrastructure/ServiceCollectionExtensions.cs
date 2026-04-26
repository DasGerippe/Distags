using Distags.Core.Interfaces;
using Distags.Infrastructure.Implementations;
using Distags.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Distags.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistagsInfrastructure(this IServiceCollection serviceCollection, string dbConnectionString)
    {
        serviceCollection.AddDbContext<DistagsDbContext>(ConfigureOptions(dbConnectionString));
        serviceCollection.AddScoped<DbContext, DistagsDbContext>();

        serviceCollection.AddTransient<ISequentialLevelActionExecutor, SequentialLevelActionExecutor>();

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