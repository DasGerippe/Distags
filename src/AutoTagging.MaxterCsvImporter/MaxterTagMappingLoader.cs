using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterTagMappingLoader
{
    private readonly ILogger<MaxterTagMappingLoader> _Logger;
    public const string MappingFileName = "maxter-tag-mapping.json";

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public MaxterTagMappingLoader(ILogger<MaxterTagMappingLoader> logger)
    {
        _Logger = logger;
    }

    public MaxterTagMappingData Load(string tagMappingFilePath)
    {
        if (!File.Exists(tagMappingFilePath))
        {
            _Logger.LogWarning("Mapping file not found at '{filePath}'. Run 'generate-mapping' first.", tagMappingFilePath);
            return new MaxterTagMappingData();
        }

        string json = File.ReadAllText(tagMappingFilePath);
        return JsonSerializer.Deserialize<MaxterTagMappingData>(json, JsonOptions) ?? new MaxterTagMappingData();
    }

    public void Save(string path, MaxterTagMappingData data)
    {
        string json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(path, json);
    }
}
