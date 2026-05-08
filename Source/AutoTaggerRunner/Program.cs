using Distags.Core;
using Distags.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogUtils;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace AutoTaggerRunner;

internal class Program
{
    static async Task Main(string[] args)
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

                configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true,
                    reloadOnChange: true);

                if (environment.IsDevelopment())
                    configBuilder.AddUserSecrets<Program>();

                configBuilder.AddEnvironmentVariables();
            });

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSerilog(loggerConfig => loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .ConfigureConsoleAndFileSinks(nameof(AutoTaggerRunner)));

                string levelTagsDbConnectionString = context.Configuration.GetConnectionString("DistagsDatabase") ??
                                                     throw new InvalidOperationException("Connection string not configured.");
                services.AddDistagsPersistence(levelTagsDbConnectionString);
                services.AddDistagsCore();

                services.AddHttpClient<LevelTagApiClient>(httpClient =>
                {
                    string levelTagApiUri = context.Configuration["LevelTagApi:Uri"]!;
                    httpClient.BaseAddress = new Uri(levelTagApiUri);

                    string levelTagApiKey = context.Configuration["LevelTagApi:ApiKey"]!;
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", levelTagApiKey);
                });

                services.AddTransient<IAutoTaggerStateManager, AutoTaggerStateFileManager>();
                services.AddTransient<IAutoTaggerLevelProviderFactory, AutoTaggerLevelProviderFactory>();

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

            AutoTaggerRunner autoTaggerRunner = scope.ServiceProvider.GetRequiredService<AutoTaggerRunner>();
            await autoTaggerRunner.RunAutoTaggerPluginsAsync(args).ConfigureAwait(false);
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
}