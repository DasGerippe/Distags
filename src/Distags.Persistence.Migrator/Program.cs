using ConfigUtils;
using Distags.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

namespace Distags.Persistence.Migrator;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            IHostEnvironment environment = context.HostingEnvironment;

            configBuilder.AddJsonFile("appsettings.json", optional: true);
            configBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);

            if (environment.IsDevelopment())
                configBuilder.AddUserSecrets<Program>();

            configBuilder.AddEnvironmentVariables();
            configBuilder.AddKeyPerFile("/run/secrets", optional: true);
        });

        builder.ConfigureServices((context, services) =>
        {
            string connectionString = context.Configuration.GetRequiredConnectionString("DistagsDatabase");
            string migrationsAssemblyName = context.Configuration.GetRequiredValue("MigrationsAssemblyName");
            services.AddDistagsPersistence(
                connectionString,
                npgsqlOptionsAction: npgsqlOptions => npgsqlOptions.MigrationsAssembly(migrationsAssemblyName));
            services.AddDistagsCore();
        });
        IHost host = builder.Build();

        AsyncServiceScope scope = host.Services.CreateAsyncScope();
        await using ConfiguredAsyncDisposable awaitedScope = scope.ConfigureAwait(false);

        DistagsDbContext dbContext = scope.ServiceProvider.GetRequiredService<DistagsDbContext>();

        Console.WriteLine("Applying EF Core migrations...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Done!");
    }
}
