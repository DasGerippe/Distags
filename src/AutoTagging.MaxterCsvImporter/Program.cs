using Distags.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AutoTagging.MaxterCsvImporter;

internal class Program
{
    private static ILogger<Program>? _Logger;

    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || args[0] != "generate-mapping")
        {
            Console.WriteLine("Usage: dotnet run --project AutoTagging.MaxterCsvImporter -- generate-mapping");
            return 1;
        }

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _Logger = loggerFactory.CreateLogger<Program>();

        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                             ?? "Production";
        string baseDirectory = AppContext.BaseDirectory;
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(baseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();

        string apiUri = configuration["DistagsApi:Uri"]
            ?? throw new InvalidOperationException("DistagsApi:Uri is not configured in appsettings.json.");

        using HttpClient httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(apiUri.TrimEnd('/'));

        List<TagDto> tags = await FetchTagsAsync(httpClient);

        string csvPath = Path.Combine(baseDirectory, "maxter_tagging_all_levels.csv");
        MaxterCsvRepository repository = new MaxterCsvRepository(csvPath, loggerFactory.CreateLogger<MaxterCsvRepository>());

        MaxterTagMappingLoader tagMappingLoader =
            new MaxterTagMappingLoader(loggerFactory.CreateLogger<MaxterTagMappingLoader>());

        string mappingFilePath = Path.Combine(baseDirectory, MaxterTagMappingLoader.MappingFileName);
        MaxterTagMappingData mapping = tagMappingLoader.Load(mappingFilePath);

        MappingGenerator tagMappingGenerator = new MappingGenerator(loggerFactory.CreateLogger<MappingGenerator>());
        tagMappingGenerator.AddMissingTokens(mapping, repository.GetAllRows(), tags);

        tagMappingLoader.Save(mappingFilePath, mapping);
        _Logger.LogDebug("Mapping written to '{mappingFilePath}'.", mappingFilePath);

        return 0;
    }

    private static async Task<List<TagDto>> FetchTagsAsync(HttpClient httpClient)
    {
        _Logger!.LogDebug("Fetching current tags from '{distagsApiBaseAddress}tags'...", httpClient.BaseAddress);

        HttpResponseMessage response = await httpClient.GetAsync("tags");
        response.EnsureSuccessStatusCode();
        List<TagDto> tags = await response.Content.ReadFromJsonAsync<List<TagDto>>()
                             ?? throw new Exception("Could not deserialize response.");

        _Logger!.LogDebug("Got {tagCount} tags.", tags.Count);

        return tags;
    }
}
