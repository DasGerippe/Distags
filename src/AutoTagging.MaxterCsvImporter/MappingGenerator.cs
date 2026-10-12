using Distags.Contracts;
using Microsoft.Extensions.Logging;

namespace AutoTagging.MaxterCsvImporter;

/// <summary>
/// Scans the CSV for tokens not yet present in the mapping file and adds them - auto-suggesting a
/// Tag when there's an unambiguous DisplayName match, otherwise leaving the value null for the user
/// to fill in by hand. Never overwrites or removes an existing key, so hand-curated entries survive reruns.
/// </summary>
public class MappingGenerator
{
    private readonly ILogger<MappingGenerator> _Logger;

    public MappingGenerator(ILogger<MappingGenerator> logger)
    {
        _Logger = logger;
    }

    public void AddMissingTokens(MaxterTagMappingData mapping, IEnumerable<MaxterCsvRow> rows, IReadOnlyList<TagDto> tags)
    {
        List<MaxterCsvRow> rowList = rows.ToList();

        AddMissingTokensForColumn(mapping.Difficulty, rowList.Select(row => row.Difficulty), tags);
        AddMissingTokensForColumn(mapping.WrRouteDif, rowList.Select(row => row.WrRouteDif), tags);
        AddMissingTokensForColumn(mapping.Theme, rowList.Select(row => row.Theme), tags);
        AddMissingTokensForColumn(mapping.DrivingStyle, rowList.Select(row => row.DrivingStyle), tags);
        AddMissingTokensForColumn(mapping.MapFeatures, rowList.Select(row => row.MapFeatures), tags);
        AddMissingTokensForColumn(mapping.WrTech, rowList.Select(row => row.WrTech), tags);
        AddMissingTokensForColumn(mapping.Readability, rowList.Select(row => row.Readability), tags);

        ResolveCarAbilities(mapping, tags);
        SeedLengthBucketsIfEmpty(mapping);
    }

    private void AddMissingTokensForColumn(
        Dictionary<string, string?> table,
        IEnumerable<string?> cellValues,
        IReadOnlyList<TagDto> tags)
    {
        IEnumerable<string> distinctTokens = cellValues
            .Where(cell => !string.IsNullOrWhiteSpace(cell))
            .SelectMany(cell => cell!.Split(','))
            .Select(token => token.Trim())
            .Where(token => token.Length > 0)
            .Distinct();

        foreach (string token in distinctTokens)
        {
            if (table.ContainsKey(token))
                continue;

            TagDto? match = tags.FirstOrDefault(tag => string.Equals(tag.DisplayName, token, StringComparison.OrdinalIgnoreCase));
            table[token] = match?.Name;

            if (match == null)
                _Logger.LogInformation("New unmapped token '{token}' - add it to the mapping file by hand.", token);
            else
                _Logger.LogInformation("New token '{token}' auto-mapped to '{match.Name}'.", token, match.Name);
        }
    }

    private void ResolveCarAbilities(MaxterTagMappingData mapping, IReadOnlyList<TagDto> tags)
    {
        HashSet<string> liveTagNames = tags.Select(tag => tag.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string ability in new[] { "jump", "wings", "grip", "boost" })
        {
            if (!mapping.CarAbilities.TryGetValue(ability, out MaxterAbilityTagNames? tagNames))
            {
                tagNames = new MaxterAbilityTagNames();
                mapping.CarAbilities[ability] = tagNames;
            }

            tagNames.Enabled = ResolveIfLiveTagExists(liveTagNames, $"{ability}-enabled");
            tagNames.Disabled = ResolveIfLiveTagExists(liveTagNames, $"{ability}-disabled");
        }
    }

    private string? ResolveIfLiveTagExists(HashSet<string> liveTagNames, string candidateTagName)
    {
        if (liveTagNames.Contains(candidateTagName))
            return candidateTagName;

        _Logger.LogDebug("Tag '{tagName}' does not exist on the server - leaving it unmapped.", candidateTagName);
        return null;
    }

    private void SeedLengthBucketsIfEmpty(MaxterTagMappingData mapping)
    {
        if (mapping.LengthBuckets.Count == 0)
            mapping.LengthBuckets = CreateDefaultBuckets(string.Empty);

        if (mapping.WrLengthBuckets.Count == 0)
            mapping.WrLengthBuckets = CreateDefaultBuckets("wr-");
    }

    private List<MaxterLengthBucket> CreateDefaultBuckets(string prefix)
    {
        return
        [
            new MaxterLengthBucket { MaxMinutes = 0.5, Tag = $"{prefix}30-sec" },
            new MaxterLengthBucket { MaxMinutes = 1, Tag = $"{prefix}1-min" },
            new MaxterLengthBucket { MaxMinutes = 2, Tag = $"{prefix}2-min" },
            new MaxterLengthBucket { MaxMinutes = 3, Tag = $"{prefix}3-min" },
            new MaxterLengthBucket { MaxMinutes = 5, Tag = $"{prefix}5-min" },
            new MaxterLengthBucket { MaxMinutes = 8, Tag = $"{prefix}8-min" },
            new MaxterLengthBucket { MaxMinutes = 15, Tag = $"{prefix}15-min" },
            new MaxterLengthBucket { MaxMinutes = 45, Tag = $"{prefix}45-min" },
            new MaxterLengthBucket { MaxMinutes = 999999, Tag = $"{prefix}hours" },
        ];
    }
}
