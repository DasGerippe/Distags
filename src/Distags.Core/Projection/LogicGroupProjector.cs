using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class LogicGroupProjector
{
    public static partial IQueryable<LogicGroupDto> SelectDto(this IQueryable<LogicGroup> logicGroups);
}
