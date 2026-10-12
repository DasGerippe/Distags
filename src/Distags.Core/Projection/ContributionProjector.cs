using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ContributionProjector
{
    public static partial IQueryable<ContributionDto> SelectDto(this IQueryable<Contribution> contributions);

    [MapDerivedType<OfficialLevel, OfficialLevelDto>]
    [MapDerivedType<WorkshopLevel, WorkshopLevelDto>]
    private static partial LevelDto ProjectLevel(Level level);

    private static string ProjectTag(Tag tag) => tag.Name;
}
