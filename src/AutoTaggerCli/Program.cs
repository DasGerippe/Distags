using AutoTagging;
using AutoTagging.Abstractions;
using Distags.Core;
using Distags.Persistence;
using Microsoft.Extensions.Configuration;
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

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                IHostEnvironment environment = context.HostingEnvironment;

                configBuilder.AddJsonFile("appsettings.json", optional: false);
                configBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);

                if (environment.IsDevelopment())
                    configBuilder.AddUserSecrets<Program>();

                configBuilder.AddEnvironmentVariables();
                configBuilder.AddKeyPerFile("/run/secrets", optional: true);
            });

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSerilog(loggerConfig => loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .ConfigureSinks<Program>(writeToFile: context.HostingEnvironment.IsDevelopment()));

                string distagsDbConnectionString = context.Configuration.GetConnectionString("DistagsDatabase") ??
                                                   throw new InvalidOperationException("Connection string not configured.");
                services.AddDistagsPersistence(distagsDbConnectionString);
                services.AddDistagsCore();

                services.AddAutoTagging();
                services.AddAutoTaggers(args);

                services.AddTransient<Program>();
            });

            hostBuilder.ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
            });

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
