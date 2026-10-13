using Distags.Api.Contracts.Users;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
public static partial class UserMapper
{
    public static partial UserResponse Map(UserDto user);
}
