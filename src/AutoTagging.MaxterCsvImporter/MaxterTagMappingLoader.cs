using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterTagMappingLoader
{
    private readonly JsonSerializerOptions _JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    private readonly ILogger<MaxterTagMappingLoader> _Logger;

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

        using FileStream fileStream = new FileStream(tagMappingFilePath, FileMode.Open, FileAccess.Read);
        MaxterTagMappingData? tagMapping = JsonSerializer.Deserialize<MaxterTagMappingData>(fileStream, _JsonOptions);

        if (tagMapping == null)
        {
            _Logger.LogWarning("Could not deserialize content of '{filePath}'.", tagMappingFilePath);
            return new MaxterTagMappingData();
        }

        return  tagMapping;
    }

    public void Save(string tagMappingFilePath, MaxterTagMappingData data)
    {
        using FileStream fileStream = new FileStream(tagMappingFilePath, FileMode.Create, FileAccess.Write);
        JsonSerializer.SerializeAsync(fileStream, data, _JsonOptions);
    }
}
