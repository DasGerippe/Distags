using Distags.Contracts;
using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class UserBanMapper : IEntityMapper<UserBan, UserBanDto>
{
    public Expression<Func<UserBan, UserBanDto>> Expression => userBan => new UserBanDto
    {
        SteamId = userBan.SteamId,
        SteamName = null, //userBan.User!.SteamName,
        SteamAvatarHash = null, //userBan.User!.SteamAvatarHash,
        BannedOn = userBan.BannedOn,
        Banner = new UserBanBannerDto
        {
            SteamId = userBan.Banner.SteamId,
            SteamName = userBan.Banner.SteamName,
            SteamAvatarHash = userBan.Banner.SteamAvatarHash,
        },
        Reason = userBan.Reason,
    };
}