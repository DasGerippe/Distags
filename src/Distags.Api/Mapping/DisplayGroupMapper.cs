using Distags.Api.Contracts.DisplayGroups;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
public static partial class DisplayGroupMapper
{
    public static partial DisplayGroupResponse Map(DisplayGroupDto displayGroup);
}
