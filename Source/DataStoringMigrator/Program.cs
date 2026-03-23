using DataStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

namespace DataStoringMigrator;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddJsonFile("appsettings.json", optional: false);

            if (context.HostingEnvironment.IsDevelopment())
                configBuilder.AddUserSecrets<Program>();
        });

        builder.ConfigureServices((context, services) =>
        {
            string connectionString = context.Configuration.GetConnectionString("LevelTagsDatabase")
                                      ?? throw new Exception("Connection string missing.");
            services.AddPostgreSqlLevelTagDbContext(connectionString);
        });

        IHost host = builder.Build();

        AsyncServiceScope scope = host.Services.CreateAsyncScope();
        await using ConfiguredAsyncDisposable awaitedScope = scope.ConfigureAwait(false);

        LevelTagDbContext dbContext = scope.ServiceProvider.GetRequiredService<LevelTagDbContext>();

        Console.WriteLine("Applying EF Core migrations...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Done!");
    }
}