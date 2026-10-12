using Distags.Api.Contracts.LogicGroups;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
public static partial class LogicGroupMapper
{
    public static partial LogicGroupResponse Map(LogicGroupDto logicGroup);
}
