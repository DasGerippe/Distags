using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Projection;

public class UserProjector : IEntityProjector<User, UserDto>
{
    public Expression<Func<User, UserDto>> Projection => user => new UserDto
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