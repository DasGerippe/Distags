using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UserProjector
{
    public static partial IQueryable<UserDto> SelectDto(this IQueryable<User> users);
}
