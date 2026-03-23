using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SteamWebApi;
using System.Runtime.CompilerServices;

namespace LevelDataPopulator;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            IHostEnvironment environment = context.HostingEnvironment;

            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (environment.IsDevelopment())
                configBuilder.AddUserSecrets<Program>();

            configBuilder.AddEnvironmentVariables();
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSteamWebApi("SteamWebApi:Settings");
            services.AddTransient<LevelDataPopulator>();
        });

        hostBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
        });

        IHost host = hostBuilder.Build();

        ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogDebug("Host setup completed. Starting application.");

        AsyncServiceScope scope = host.Services.CreateAsyncScope();
        await using ConfiguredAsyncDisposable awaitedScope = scope.ConfigureAwait(false);

        try
        {
            LevelDataPopulator levelDataPopulator = scope.ServiceProvider.GetRequiredService<LevelDataPopulator>();
            await levelDataPopulator.SyncLevelsFromSteamApiAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled error occured during application lifetime.");
        }

        logger.LogDebug("End of application.");
    }
}