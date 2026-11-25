using AppHostUtils;
using AutoTagging;
using AutoTagging.Abstractions;
using Distags.Core;
using Distags.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogUtils;
using System.Runtime.CompilerServices;

namespace AutoTaggerCli;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ConfigureBootstrapLogger()
            .CreateBootstrapLogger();

        try
        {
            Log.Debug("Setting up application host...");

            HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder();

            hostBuilder.Configuration.AddDockerSecrets();
            hostBuilder.Configuration.ReplaceConnectionStringPassword("DistagsDatabase");

            hostBuilder.Logging.ClearProviders();
            hostBuilder.Services.AddSerilog(loggerConfig => loggerConfig
                .ReadFrom.Configuration(hostBuilder.Configuration)
                .ConfigureSinks<Program>(writeToFile: hostBuilder.Environment.IsDevelopment()));

            hostBuilder.Services.AddDistagsCore();
            hostBuilder.Services.AddDistagsPersistence(hostBuilder.Configuration.GetRequiredConnectionString("DistagsDatabase"));

            hostBuilder.Services.AddAutoTagging();
            hostBuilder.Services.AddAutoTaggers(args);

            hostBuilder.Services.AddTransient<Program>();

            IHost host = hostBuilder.Build();

            Log.Debug("Application host setup completed.");

            AsyncServiceScope scope = host.Services.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);

            IHostApplicationLifetime hostLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
            Program program = scope.ServiceProvider.GetRequiredService<Program>();
            await program.Run(hostLifetime.ApplicationStopping).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred causing the Host to terminate.");
        }
        finally
        {
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
    }

    private readonly AutoTaggerSeeder _AutoTaggerSeeder;
    private readonly AutoTaggerHost _AutoTaggerHost;
    private readonly IEnumerable<IAutoTagger> _AutoTaggers;
    private readonly ILogger<Program> _Logger;

    public Program(
        AutoTaggerSeeder autoTaggerSeeder,
        AutoTaggerHost autoTaggerHost,
        IEnumerable<IAutoTagger> autoTaggers,
        ILogger<Program> logger)
    {
        _AutoTaggerSeeder = autoTaggerSeeder;
        _AutoTaggerHost = autoTaggerHost;
        _AutoTaggers = autoTaggers;
        _Logger = logger;
    }

    private async Task Run(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Starting application...");

        await _AutoTaggerSeeder.CreateOrUpdateRegisteredAutoTaggers(cancellationToken);

        foreach (IAutoTagger autoTagger in _AutoTaggers)
        {
            try
            {
                await _AutoTaggerHost.ExecuteAsync(autoTagger, cancellationToken);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "An error occurred while running {autoTagger}.", autoTagger.Name);
            }
        }

        _Logger.LogDebug("End of application.");
    }
}
