using Microsoft.Extensions.Logging;
using System.Globalization;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterTagResolver
{
    private readonly ILogger<MaxterTagResolver> _Logger;

    public MaxterTagResolver(ILogger<MaxterTagResolver> logger)
    {
        _Logger = logger;
    }

    public HashSet<string> ResolveDesiredTags(MaxterCsvRow row, MaxterTagMappingData mapping)
    {
        HashSet<string> desired = new HashSet<string>();

        AddMapped(desired, row.Difficulty, mapping.Difficulty);
        AddMapped(desired, row.WrRouteDif, mapping.WrRouteDif);
        AddMapped(desired, row.Theme, mapping.Theme);
        AddMapped(desired, row.DrivingStyle, mapping.DrivingStyle);
        AddMapped(desired, row.MapFeatures, mapping.MapFeatures);
        AddMapped(desired, row.WrTech, mapping.WrTech);
        AddMapped(desired, row.Readability, mapping.Readability);

        AddCarAbilities(desired, row.CarAbilities, mapping.CarAbilities);

        AddLengthBucket(desired, row.Length, mapping.LengthBuckets);
        AddLengthBucket(desired, row.WrLength, mapping.WrLengthBuckets);

        return desired;
    }

    private void AddMapped(HashSet<string> desired, string? cellValue, IReadOnlyDictionary<string, string?> table)
    {
        foreach (string token in SplitTokens(cellValue))
        {
            if (!table.TryGetValue(token, out string? tagName))
            {
                _Logger.LogWarning("Unmapped token '{token}' - run 'generate-mapping' and fill it in. Skipping.", token);
                continue;
            }

            if (!string.IsNullOrEmpty(tagName))
                desired.Add(tagName);
        }
    }

    private void AddCarAbilities(HashSet<string> desired, string? cellValue, IReadOnlyDictionary<string, MaxterAbilityTagNames> abilities)
    {
        List<string> tokens = SplitTokens(cellValue).ToList();
        if (tokens.Count == 0)
            return;

        // Which single ability toggles is unspecified in the sheet - never guess, leave all abilities untouched.
        if (tokens.Any(token => string.Equals(token, "Toggles", StringComparison.OrdinalIgnoreCase)))
            return;

        foreach ((string ability, MaxterAbilityTagNames tagNames) in abilities)
        {
            bool present = tokens.Any(token => string.Equals(token, ability, StringComparison.OrdinalIgnoreCase));
            string? tagName = present ? tagNames.Enabled : tagNames.Disabled;
            if (!string.IsNullOrEmpty(tagName))
                desired.Add(tagName);
        }
    }

    private void AddLengthBucket(HashSet<string> desired, string? cellValue, IReadOnlyList<MaxterLengthBucket> buckets)
    {
        if (buckets.Count == 0)
            return;

        List<double> minuteValues = SplitTokens(cellValue)
            .Select(TryParseMinutes)
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .ToList();

        if (minuteValues.Count == 0)
            return;

        double maxMinutes = minuteValues.Max();
        MaxterLengthBucket bucket = buckets
            .OrderBy(b => b.MaxMinutes)
            .FirstOrDefault(b => b.MaxMinutes >= maxMinutes)
            ?? buckets.OrderByDescending(b => b.MaxMinutes).First();

        if (!string.IsNullOrEmpty(bucket.Tag))
            desired.Add(bucket.Tag);
    }

    private double? TryParseMinutes(string token)
    {
        string normalized = token.Trim().ToLowerInvariant().Replace("+", string.Empty).Trim();

        double multiplier;
        if (normalized.EndsWith("sec", StringComparison.Ordinal))
        {
            multiplier = 1.0 / 60.0;
            normalized = normalized[..^3].Trim();
        }
        else if (normalized.EndsWith("min", StringComparison.Ordinal))
        {
            multiplier = 1.0;
            normalized = normalized[..^3].Trim();
        }
        else if (normalized.EndsWith("h", StringComparison.Ordinal))
        {
            multiplier = 60.0;
            normalized = normalized[..^1].Trim();
        }
        else
        {
            return null;
        }

        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            ? value * multiplier
            : null;
    }

    private IEnumerable<string> SplitTokens(string? cellValue)
    {
        return string.IsNullOrWhiteSpace(cellValue)
            ? []
            : cellValue.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }
}
