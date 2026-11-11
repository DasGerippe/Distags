using AppHostUtils;
using Distags.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogUtils;
using System.Runtime.CompilerServices;

namespace Distags.Persistence.Migrator;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ConfigureBootstrapLogger()
            .CreateBootstrapLogger();

        try
        {
            Log.Debug("Setting up application host...");

            HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

            hostBuilder.Configuration.AddDockerSecrets();
            hostBuilder.Configuration.ReplaceConnectionStringPassword("DistagsDatabase");

            hostBuilder.Logging.ClearProviders();
            hostBuilder.Services.AddSerilog(loggerConfig => loggerConfig
                .ReadFrom.Configuration(hostBuilder.Configuration)
                .ConfigureSinks<Program>(writeToFile: hostBuilder.Environment.IsDevelopment()));

            string connectionString = hostBuilder.Configuration.GetRequiredConnectionString("DistagsDatabase");
            hostBuilder.Services.AddDistagsPersistence(
                connectionString,
                npgsqlOptionsAction: npgsqlOptions =>
                    npgsqlOptions.MigrationsAssembly($"{nameof(Distags)}.{nameof(Persistence)}.{nameof(Migrations)}"));
            hostBuilder.Services.AddDistagsCore();

            IHost host = hostBuilder.Build();

            AsyncServiceScope scope = host.Services.CreateAsyncScope();
            await using ConfiguredAsyncDisposable awaitedScope = scope.ConfigureAwait(false);

            DistagsDbContext dbContext = scope.ServiceProvider.GetRequiredService<DistagsDbContext>();
            IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string? targetMigrationName = config["TargetMigration"];
            if (string.IsNullOrEmpty(targetMigrationName))
            {
                Log.Debug("Migrating database to latest migration...");
                await dbContext.Database.MigrateAsync();
            }
            else
            {
                Log.Debug("Migrating database to migration {migration}...", targetMigrationName);
                await dbContext.Database.MigrateAsync(targetMigrationName);
            }
            Log.Debug("Done!");

            return 0;
        }
        catch (Exception ex) when (ex is not HostAbortedException)
        {
            Log.Fatal(ex, "An unexpected error occurred causing the Host to terminate.");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
