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
            hostBuilder.Logging.AddConsole();

            hostBuilder.Services.AddSerilog(loggerConfig => loggerConfig
                .ReadFrom.Configuration(hostBuilder.Configuration)
                .ConfigureSinks<Program>(writeToFile: hostBuilder.Environment.IsDevelopment()));

            hostBuilder.Services.AddDistagsPersistence(hostBuilder.Configuration.GetRequiredConnectionString("DistagsDatabase"));
            hostBuilder.Services.AddDistagsCore();

            hostBuilder.Services.AddAutoTagging();
            hostBuilder.Services.AddAutoTaggers(args);

            hostBuilder.Services.AddTransient<Program>();

            IHost host = hostBuilder.Build();

            Log.Debug("Application host setup completed.");

            AsyncServiceScope scope = host.Services.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);

            Program program = scope.ServiceProvider.GetRequiredService<Program>();
            await program.Run().ConfigureAwait(false);
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

    private readonly AutoTaggerRunner _AutoTaggerRunner;
    private readonly IEnumerable<AutoTagger> _AutoTaggers;
    private readonly ILogger<Program> _Logger;

    public Program(AutoTaggerRunner autoTaggerRunner, IEnumerable<AutoTagger> autoTaggers, ILogger<Program> logger)
    {
        _AutoTaggerRunner = autoTaggerRunner;
        _AutoTaggers = autoTaggers;
        _Logger = logger;
    }

    private async Task Run()
    {
        _Logger.LogDebug("Starting application...");

        foreach (AutoTagger autoTagger in _AutoTaggers)
        {
            try
            {
                await _AutoTaggerRunner.RunAsync(autoTagger);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "An error occurred while running {autoTagger}.", autoTagger.Name);
            }
        }

        _Logger.LogDebug("End of application.");
    }
}
