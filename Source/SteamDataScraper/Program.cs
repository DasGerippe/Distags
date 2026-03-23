using DataStoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SteamWebApi;
using System.Runtime.CompilerServices;

namespace SteamDataScraper;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            IHostEnvironment environment = context.HostingEnvironment;

            configBuilder.AddJsonFile("appsettings.json", optional: false);
            configBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);

            if (environment.IsDevelopment())
                configBuilder.AddUserSecrets<Program>();

            configBuilder.AddEnvironmentVariables();
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSteamWebApi("SteamWebApi");

            services.AddOptions<SteamDataScraperSettings>()
                .BindConfiguration("SteamDataScraper")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            string levelTagsDbConnectionString = context.Configuration.GetConnectionString("LevelTagsDatabase") ??
                                                 throw new InvalidOperationException("Connection string not configured.");
            services.AddPostgreSqlLevelTagDbContext(levelTagsDbConnectionString);

            services.AddTransient<LevelScraper>();
            services.AddTransient<LevelCreatorScraper>();
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
            LevelScraper levelScraper = scope.ServiceProvider.GetRequiredService<LevelScraper>();
            await levelScraper.ScrapeLevelsAsync().ConfigureAwait(false);

            LevelCreatorScraper levelCreatorScraper = scope.ServiceProvider.GetRequiredService<LevelCreatorScraper>();
            await levelCreatorScraper.ScrapeOutdatedLevelCreatorsAsync().ConfigureAwait(false);
            await levelCreatorScraper.ScrapeUnknownLevelCreatorsAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled error occured during application lifetime.");
        }

        logger.LogDebug("End of application.");
    }
}