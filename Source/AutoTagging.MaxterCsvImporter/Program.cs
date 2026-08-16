using System.Net.Http.Json;
using Distags.Contracts;
using Microsoft.Extensions.Configuration;

namespace AutoTagging.MaxterCsvImporter;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || args[0] != "generate-mapping")
        {
            Console.WriteLine("Usage: dotnet run --project AutoTagging.MaxterCsvImporter -- generate-mapping");
            return 1;
        }

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

        using HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiUri.TrimEnd('/') + "/"),
        };

        Console.WriteLine($"Fetching current tags from '{httpClient.BaseAddress}tags'...");
        List<TagDto> tags = await FetchTagsAsync(httpClient);
        Console.WriteLine($"Got {tags.Count} tags.");

        string csvPath = Path.Combine(baseDirectory, "maxter_tagging_all_levels.csv");
        MaxterCsvRepository repository = new MaxterCsvRepository(csvPath);

        string mappingPath = Path.Combine(baseDirectory, MaxterTagMappingLoader.MappingFileName);
        MaxterTagMappingData mapping = MaxterTagMappingLoader.Load(mappingPath);

        MappingGenerator.AddMissingTokens(mapping, repository.GetAllRows(), tags);

        MaxterTagMappingLoader.Save(mappingPath, mapping);
        Console.WriteLine($"Mapping written to '{mappingPath}'.");

        return 0;
    }

    private static async Task<List<TagDto>> FetchTagsAsync(HttpClient httpClient)
    {
        HttpResponseMessage response = await httpClient.GetAsync("tags");
        response.EnsureSuccessStatusCode();
        List<TagDto>? tags = await response.Content.ReadFromJsonAsync<List<TagDto>>();
        return tags ?? [];
    }
}
