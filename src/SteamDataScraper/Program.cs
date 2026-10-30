using ConfigUtils;
using Distags.Core;
using Distags.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SerilogUtils;
using SteamDataScraper.Creators;
using SteamDataScraper.Levels;
using SteamWebApi;
using System.Runtime.CompilerServices;

namespace SteamDataScraper;

internal class Program
{
    static async Task Main(string[] args)
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

            hostBuilder.Services.AddSteamWebApi("SteamWebApi");

            hostBuilder.Services.AddOptions<CreatorRefreshSettings>()
                .BindConfiguration($"{nameof(SteamDataScraper)}:{nameof(CreatorRefreshSettings)}")
                .ValidateDataAnnotations()
                .ValidateOnStart();
            hostBuilder.Services.AddTransient<CreatorRefreshSettings>(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<CreatorRefreshSettings>>().Value);

            hostBuilder.Services.AddDistagsPersistence(hostBuilder.Configuration.GetRequiredConnectionString("DistagsDatabase"));
            hostBuilder.Services.AddDistagsCore();

            hostBuilder.Services.AddTransient<Program>();
            hostBuilder.Services.AddTransient<LevelScraper>();
            hostBuilder.Services.AddTransient<CreatorScraper>();
            hostBuilder.Services.AddTransient<ICreatorRefreshDateManager, CreatorRefreshDateDatabaseManager>();
            hostBuilder.Services.AddTransient<ICreatorRefreshCountCalculator, CreatorRefreshCountCalculator>();

            IHost host = hostBuilder.Build();

            Log.Debug("Application host setup completed.");

            AsyncServiceScope scope = host.Services.CreateAsyncScope();
            await using ConfiguredAsyncDisposable awaitedScope = scope.ConfigureAwait(false);

            Program program = scope.ServiceProvider.GetRequiredService<Program>();
            await program.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred causing the Host to terminate.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
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
            await _CreatorScraper.ScrapeLevelCreatorsAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An unhandled error occured during application lifetime.");
        }

        _Logger.LogDebug("End of application.");
    }
}
