using DataClasses;
using Distags.Contracts;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class UserMapper : IEntityMapper<User, UserDto>
{
    public Expression<Func<User, UserDto>> Expression => user => new UserDto
    {
        SteamId = user.SteamId ?? -1,
        SteamName = user.SteamName,
        SteamAvatarHash = user.SteamAvatarHash,
        AcceptsGuidelines = user.AcceptsGuidelines,
        IsAnonymous = user.IsAnonymous,
        IsBanned = user.IsBanned,
        Roles = user.Roles,
    };
}