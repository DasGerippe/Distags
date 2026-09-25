using Distags.Api.Contracts.Levels;
using Distags.Core.Filtering.Levels;
using Distags.Core.Queries.BrowseLevels;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper(
    RequiredMappingStrategy = RequiredMappingStrategy.Target,
    RequiredEnumMappingStrategy = RequiredMappingStrategy.Source,
    EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class BrowseLevelsMapper
{
    [MapPropertyFromSource(nameof(BrowseLevelsQuery.FilterParameters))]
    public static partial BrowseLevelsQuery Map(BrowseLevelsRequest request);

    [MapperIgnoreTarget(nameof(LevelFilterParameters.Type))]
    private static partial LevelFilterParameters MapToLevelFilterParameters(BrowseLevelsRequest request);

    private static List<string> GetNormalizedTagNames(string? commaSeparatedTags)
    {
        if (string.IsNullOrEmpty(commaSeparatedTags))
            return [];

        string[] tagNames = commaSeparatedTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        List<string> normalizedTagNames = tagNames.Select(tag => tag.ToLowerInvariant()).ToList();
        return normalizedTagNames;
    }
}
