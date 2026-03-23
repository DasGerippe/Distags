using DataClasses;
using DataStoring.Contract;
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

        services.AddScoped<IRepository<TagGroup>, Repository<TagGroup, LevelTagDbContext>>();
        services.AddScoped<IRepository<Tag>, Repository<Tag, LevelTagDbContext>>();
        services.AddScoped<IRepository<Level>, Repository<Level, LevelTagDbContext>>();
        services.AddScoped<IRepository<LevelTag>, Repository<LevelTag, LevelTagDbContext>>();
        services.AddScoped<IRepository<Creator>, Repository<Creator, LevelTagDbContext>>();

        return services;
    }
}