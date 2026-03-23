using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataStoring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNpgsqlLevelTagDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LevelTagDbContext>(ConfigureNpgsqlOptions(connectionString));
        return services;
    }

    public static IServiceCollection AddNpgsqlLevelTagDbContextFactory(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<LevelTagDbContext>(ConfigureNpgsqlOptions(connectionString));
        return services;
    }

    private static Action<DbContextOptionsBuilder> ConfigureNpgsqlOptions(string connectionString)
    {
        return options => options
            .UseNpgsql(connectionString, dbOptions => dbOptions.MigrationsAssembly("DataStoringMigrations"))
            .UseSnakeCaseNamingConvention();
    }
}