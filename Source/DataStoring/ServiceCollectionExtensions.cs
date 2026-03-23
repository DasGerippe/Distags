using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataStoring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlLevelTagDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LevelTagDbContext>(options => options
            .UseNpgsql(connectionString, dbOptions => dbOptions.MigrationsAssembly("DataStoringMigrations"))
            .UseSnakeCaseNamingConvention());

        return services;
    }
}