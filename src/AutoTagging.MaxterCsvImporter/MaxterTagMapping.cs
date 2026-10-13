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
