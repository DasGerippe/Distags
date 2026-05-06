using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.PublishedFileService;
using SteamWebApi.Dtos.SteamUser;
using System.Runtime.CompilerServices;

namespace TestProject;

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
            services.AddSteamWebApi(nameof(SteamWebApiSettings));

            services.AddTransient<TestClass>();
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
            TestClass testClass = host.Services.GetRequiredService<TestClass>();
            await testClass.DoStuff();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled error occured during application lifetime.");
        }

        logger.LogDebug("End of application.");
    }

    private class TestClass
    {
        private readonly IConfiguration _Configuration;
        private readonly IPublishedFileService _PublishedFileService;
        private readonly ISteamUser _SteamUser;

        public TestClass(IConfiguration configuration, IPublishedFileService publishedFileService, ISteamUser steamUser)
        {
            _Configuration = configuration;
            _PublishedFileService = publishedFileService;
            _SteamUser = steamUser;
        }

        public async Task DoStuff()
        {
            //QueryFileParameters queryFilesParameters = new QueryFileParameters
            //{
            //    Key = _Configuration["SteamWebApi:Key"],
            //    AppId = 233610,
            //    QueryType = QueryType.RankedByPublicationDate,
            //    ItemsPerPage = 10,
            //    ReturnMetadata = true,
            //};

            //int i = 0;
            //await foreach (PublishedFileDetail file in _PublishedFileService.QueryFilesAsync(queryFilesParameters).ConfigureAwait(false))
            //{
            //    Console.WriteLine($"{file.PublishedFileId,-20}{file.Title}");
            //    if (i++ >= 20)
            //        break;
            //}

            GetPlayerSummariesParameters getPlayerSummariesParameters = new GetPlayerSummariesParameters
            {
                Key = _Configuration["SteamWebApi:Key"],
                SteamIds =
                [
                    76561198032726698,
                    76561197985358612,
                    76561198139613473,
                    76561198107351346,
                    76561198073516568,
                    76561198006359986,
                    76561197999682427,
                ],
            };

            await foreach (PlayerSummary player in _SteamUser.GetPlayerSummariesAsync(getPlayerSummariesParameters).ConfigureAwait(false))
            {
                Console.WriteLine($"{player.SteamId,-20}{player.PersonaName}");
            }
        }
    }
}