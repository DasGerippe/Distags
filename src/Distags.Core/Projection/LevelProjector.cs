using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class LevelProjector
{
    public static partial IQueryable<LevelDto> SelectDto(this IQueryable<Level> levels);

    public static partial IQueryable<OfficialLevelDto> SelectDto(this IQueryable<OfficialLevel> officialLevels);

    public static partial IQueryable<WorkshopLevelDto> SelectDto(this IQueryable<WorkshopLevel> workshopLevels);

    [MapDerivedType<OfficialLevel, OfficialLevelDto>]
    [MapDerivedType<WorkshopLevel, WorkshopLevelDto>]
    private static partial LevelDto ProjectLevel(Level level);

    private static string ProjectTag(Tag tag) => tag.Name;
}
