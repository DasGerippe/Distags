using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Projection;

public class UserBanProjector : IEntityProjector<UserBan, UserBanDto>
{
    public Expression<Func<UserBan, UserBanDto>> Projection => userBan => new UserBanDto
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