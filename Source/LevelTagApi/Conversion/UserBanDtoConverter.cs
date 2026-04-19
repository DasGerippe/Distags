using DataClasses;
using DataStoring;
using LevelTagApi.Dtos.UserBans;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Conversion;

public class UserBanDtoConverter
{
    private readonly LevelTagDbContext _DbContext;

    public UserBanDtoConverter(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public IQueryable<UserBan> IncludeRequiredNavigations(IQueryable<UserBan> userBans)
    {
        return userBans.Include(userBan => userBan.Banner);
    }

    public IQueryable<UserBanDto> SelectDto(IQueryable<UserBan> userBans)
    {
        return IncludeRequiredNavigations(userBans)
            .LeftJoin(
                _DbContext.Set<User>(),
                userBan => userBan.SteamId,
                user => user.SteamId,
                (userBan, user) => new UserBanDto
                {
                    SteamId = userBan.SteamId,
                    SteamName = user!.SteamName,
                    SteamAvatarHash = user!.SteamAvatarHash,
                    BannedOn = userBan.BannedOn,
                    Banner = userBan.Banner.ToDisplayDto(),
                    Reason = userBan.Reason,
                });
    }
}