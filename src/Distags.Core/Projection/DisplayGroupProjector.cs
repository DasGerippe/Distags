using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class DisplayGroupProjector
{
    public static partial IQueryable<DisplayGroupDto> SelectDto(this IQueryable<DisplayGroup> displayGroups);
}
