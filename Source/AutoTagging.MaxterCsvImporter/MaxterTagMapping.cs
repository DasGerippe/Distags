using System.Text.Json;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterTagMappingData
{
    public Dictionary<string, string?> Difficulty { get; set; } = new();

    public Dictionary<string, string?> WrRouteDif { get; set; } = new();

    public Dictionary<string, string?> Theme { get; set; } = new();

    public Dictionary<string, string?> DrivingStyle { get; set; } = new();

    public Dictionary<string, string?> MapFeatures { get; set; } = new();

    public Dictionary<string, string?> WrTech { get; set; } = new();

    public Dictionary<string, string?> Readability { get; set; } = new();

    public Dictionary<string, MaxterAbilityTagNames> CarAbilities { get; set; } = new();

    public List<MaxterLengthBucket> LengthBuckets { get; set; } = new();

    public List<MaxterLengthBucket> WrLengthBuckets { get; set; } = new();
}

public class MaxterAbilityTagNames
{
    public string? Enabled { get; set; }

    public string? Disabled { get; set; }
}

public class MaxterLengthBucket
{
    public double MaxMinutes { get; set; }

    public string Tag { get; set; } = string.Empty;
}

public static class MaxterTagMappingLoader
{
    public const string MappingFileName = "maxter-tag-mapping.json";

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public static MaxterTagMappingData Load(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"[MaxterCsvImporter] Mapping file not found at '{path}'. Run 'generate-mapping' first.");
            return new MaxterTagMappingData();
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<MaxterTagMappingData>(json, JsonOptions) ?? new MaxterTagMappingData();
    }

    public static void Save(string path, MaxterTagMappingData data)
    {
        string json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(path, json);
    }
}
