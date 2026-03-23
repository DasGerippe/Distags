using DataStoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SteamDataScraper.Creators;
using SteamDataScraper.Levels;
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

            services.AddOptions<CreatorRefreshSettings>()
                .BindConfiguration($"{nameof(SteamDataScraper)}:{nameof(CreatorRefreshSettings)}")
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddTransient<CreatorRefreshSettings>(services =>
                services.GetRequiredService<IOptions<CreatorRefreshSettings>>().Value);

            string levelTagsDbConnectionString = context.Configuration.GetConnectionString("LevelTagsDatabase") ??
                                                 throw new InvalidOperationException("Connection string not configured.");
            services.AddPostgreSqlLevelTagDbContext(levelTagsDbConnectionString);


            services.AddTransient<Program>();
            services.AddTransient<LevelScraper>();
            services.AddTransient<CreatorScraper>();
            services.AddTransient<ICreatorRefreshDateManager, CreatorRefreshDateFileManager>();
            services.AddTransient<ICreatorRefreshCountCalculator, CreatorRefreshCountCalculator>();
        });

        hostBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
        });

        IHost host = hostBuilder.Build();

        AsyncServiceScope scope = host.Services.CreateAsyncScope();
        await using ConfiguredAsyncDisposable awaitedScope = scope.ConfigureAwait(false);

        ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogDebug("Host setup completed.");

        Program program = scope.ServiceProvider.GetRequiredService<Program>();
        await program.RunAsync();
    }

    private readonly LevelScraper _LevelScraper;
    private readonly CreatorScraper _CreatorScraper;
    private readonly ILogger<Program> _Logger;

    public Program(LevelScraper levelScraper, CreatorScraper creatorScraper, ILogger<Program> logger)
    {
        _LevelScraper = levelScraper;
        _CreatorScraper = creatorScraper;
        _Logger = logger;
    }

    public async Task RunAsync()
    {
        _Logger.LogDebug("Starting application...");

        try
        {
            await _LevelScraper.ScrapeLevelsAsync().ConfigureAwait(false);

            await _CreatorScraper.ScrapeUnknownLevelCreatorsAsync().ConfigureAwait(false);
            await _CreatorScraper.ScrapeOutdatedLevelCreatorsAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An unhandled error occured during application lifetime.");
        }

        _Logger.LogDebug("End of application.");
    }
}