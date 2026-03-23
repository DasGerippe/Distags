using DataClasses;
using DataStoring.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataStoring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryLevelTagDbContext(this IServiceCollection services)
    {
        services.AddDbContext<LevelTagDbContext>(options => options.UseInMemoryDatabase("LevelTagDatabase"));

        return services;
    }

    public static IServiceCollection AddPostgreSqlLevelTagDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LevelTagDbContext>(options => options
            .UseNpgsql(connectionString, dbOptions => dbOptions.MigrationsAssembly("DataStoringMigrations"))
            .UseSnakeCaseNamingConvention());

        services.AddTransient<IRepository<Tag>, Repository<Tag, LevelTagDbContext>>();
        services.AddTransient<IRepository<Level>, Repository<Level, LevelTagDbContext>>();
        services.AddTransient<IRepository<LevelTag>, Repository<LevelTag, LevelTagDbContext>>();
        services.AddTransient<IRepository<SteamUser>, Repository<SteamUser, LevelTagDbContext>>();

        return services;
    }

    public static IServiceCollection AddSqliteLevelTagDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LevelTagDbContext>(options => options.UseSqlite(connectionString));

        return services;
    }
}