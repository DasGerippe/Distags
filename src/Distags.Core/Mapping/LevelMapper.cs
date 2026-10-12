using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class LevelMapper
{
    [MapDerivedType<OfficialLevel, OfficialLevelDto>]
    [MapDerivedType<WorkshopLevel, WorkshopLevelDto>]
    public static partial LevelDto Map(Level level);

    private static string MapTag(Tag tag) => tag.Name;
}
