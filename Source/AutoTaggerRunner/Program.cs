using AutoTagging.Abstractions;
using AutoTagging.Abstractions.Progress;
using Distags.Core;
using Distags.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogUtils;
using System.Runtime.CompilerServices;

namespace AutoTaggerRunner;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ConfigureConsoleAndFileSinks(nameof(AutoTaggerRunner))
            .CreateBootstrapLogger();

        try
        {
            Log.Debug("Setting up application host...");

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                IHostEnvironment environment = context.HostingEnvironment;

                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ProgressDirectoryPath"] = Constants.PluginsFolderName,
                });

                configBuilder.AddJsonFile("appsettings.json", optional: false);
                configBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);

                if (environment.IsDevelopment())
                    configBuilder.AddUserSecrets<Program>();

                configBuilder.AddEnvironmentVariables();
            });

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSerilog(loggerConfig => loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .ConfigureConsoleAndFileSinks(nameof(AutoTaggerRunner)));

                string distagsDbConnectionString = context.Configuration.GetConnectionString("DistagsDatabase") ??
                                                   throw new InvalidOperationException("Connection string not configured.");
                services.AddDistagsPersistence(distagsDbConnectionString);
                services.AddDistagsCore();

                services.AddHttpClient<DistagsApiClient>(httpClient =>
                {
                    string distagsApiUri = context.Configuration["DistagsApi:Uri"]!;
                    httpClient.BaseAddress = new Uri(distagsApiUri);
                });

                services.AddTransient<IAutoTaggerProgressManager, AutoTaggerProgressFileManager>();

                services.AddTransient<AutoTaggerLoader>();
                services.AddTransient<AutoTaggerRunner>();
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
            await program.Run(args).ConfigureAwait(false);
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

    private readonly AutoTaggerLoader _AutoTaggerLoader;
    private readonly AutoTaggerRunner _AutoTaggerRunner;
    private readonly ILogger<Program> _Logger;

    public Program(AutoTaggerLoader autoTaggerLoader, AutoTaggerRunner autoTaggerRunner, ILogger<Program> logger)
    {
        _AutoTaggerLoader = autoTaggerLoader;
        _AutoTaggerRunner = autoTaggerRunner;
        _Logger = logger;
    }

    private async Task Run(string[] args)
    {
        _Logger.LogDebug("Starting application...");

        List<AutoTagger> autoTaggers = _AutoTaggerLoader.LoadAutoTaggersFromDlls(args);
        await _AutoTaggerRunner.TryRunAutoTaggersAsync(autoTaggers);

        _Logger.LogDebug("End of application.");
    }
}
