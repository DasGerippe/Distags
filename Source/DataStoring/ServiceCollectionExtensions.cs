using DataStoring.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace DataStoring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNpgsqlLevelTagDbContext(
        this IServiceCollection services,
        string connectionString,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction = null,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        return services.AddDbContext<LevelTagDbContext>(ConfigureOptions(connectionString, optionsAction, npgsqlOptionsAction));
    }

    public static IServiceCollection AddNpgsqlLevelTagDbContextFactory(
        this IServiceCollection services,
        string connectionString,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction = null,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        return services.AddDbContextFactory<LevelTagDbContext>(ConfigureOptions(connectionString, optionsAction, npgsqlOptionsAction));
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