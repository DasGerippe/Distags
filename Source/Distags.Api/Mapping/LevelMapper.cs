using Distags.Api.Contracts.Levels;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class LevelMapper
{
    [MapDerivedType<OfficialLevelDto, OfficialLevelResponse>]
    [MapDerivedType<WorkshopLevelDto, WorkshopLevelResponse>]
    [MapperIgnoreTarget(nameof(LevelResponse.Tags))]
    public static partial LevelResponse Map(LevelDto level);

    public static partial OfficialLevelResponse Map(OfficialLevelDto officialLevel);

    public static partial WorkshopLevelResponse Map(WorkshopLevelDto workshopLevel);
}
