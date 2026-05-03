using System.Linq.Expressions;
using DataClasses;
using Distags.Core.Dtos;

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