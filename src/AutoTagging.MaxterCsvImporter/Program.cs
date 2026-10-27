using Distags.Persistence.Seeding.Tags;
using Microsoft.Extensions.Logging;

namespace AutoTagging.MaxterCsvImporter;

internal class Program
{
    private static ILogger<Program>? _Logger;

    private static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] != "generate-mapping")
        {
            Console.WriteLine("Usage: dotnet run --project AutoTagging.MaxterCsvImporter -- generate-mapping");
            return 1;
        }

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _Logger = loggerFactory.CreateLogger<Program>();

        string baseDirectory = AppContext.BaseDirectory;

        List<SeedTag> tags = new SeedTagsAndGroupsProvider().GetSeedTags();

        string csvPath = Path.Combine(baseDirectory, Constants.TaggingCsvFileName);
        MaxterCsvRepository repository = new MaxterCsvRepository(csvPath, loggerFactory.CreateLogger<MaxterCsvRepository>());

        MaxterTagMappingLoader tagMappingLoader =
            new MaxterTagMappingLoader(loggerFactory.CreateLogger<MaxterTagMappingLoader>());

        string mappingFilePath = Path.Combine(baseDirectory, Constants.TagMappingJsonFileName);
        MaxterTagMappingData mapping = tagMappingLoader.Load(mappingFilePath);

        MappingGenerator tagMappingGenerator = new MappingGenerator(loggerFactory.CreateLogger<MappingGenerator>());
        tagMappingGenerator.AddMissingTokens(mapping, repository.GetAllRows(), tags);

        tagMappingLoader.Save(mappingFilePath, mapping);
        _Logger.LogInformation("Mapping written to '{mappingFilePath}'.", mappingFilePath);

        return 0;
    }
}
