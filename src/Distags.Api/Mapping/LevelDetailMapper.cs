using Distags.Api.Contracts.Levels;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class LevelDetailMapper
{
    [MapDerivedType<OfficialLevelDto, OfficialLevelResponse>]
    [MapDerivedType<WorkshopLevelDto, WorkshopLevelResponse>]
    public static partial LevelResponse Map(LevelDto level);

    [MapPropertyFromSource(nameof(LevelResponse.Key), Use = nameof(GetKey))]
    public static partial OfficialLevelResponse Map(OfficialLevelDto officialLevel);

    [MapPropertyFromSource(nameof(LevelResponse.Key), Use = nameof(GetKey))]
    public static partial WorkshopLevelResponse Map(WorkshopLevelDto workshopLevel);

    private static string GetKey(LevelDto level)
    {
        return LevelKeyGenerator.GetKey(level);
    }
}
