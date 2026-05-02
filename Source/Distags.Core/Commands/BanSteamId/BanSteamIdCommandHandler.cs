using DataClasses;
using Distags.Core.Exceptions;
using Distags.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.BanSteamId;

public class BanSteamIdCommandHandler : ICommandHandler<BanSteamIdCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly UserBanPermissionChecker _PermissionChecker;

    public BanSteamIdCommandHandler(DistagsDbContext dbContext, UserBanPermissionChecker permissionChecker)
    {
        _DbContext = dbContext;
        _PermissionChecker = permissionChecker;
    }

    public async Task HandleAsync(BanSteamIdCommand command)
    {
        if (command.SteamId == command.BannerSteamId)
            throw new CannotBanSelfException();

        User? banner = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.BannerSteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (banner == null)
            throw new UnknownUserException(command.BannerSteamId);

        bool steamIdBanned = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == command.SteamId)
            .AnyAsync()
            .ConfigureAwait(false);
        if (steamIdBanned)
            throw new SteamIdAlreadyBannedException();

        User? bannedUser = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.SteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (bannedUser != null)
        {
            if (!_PermissionChecker.CanBan(banner, bannedUser))
                throw new CannotBanOtherUserException();

            bannedUser.IsBanned = true;
            bannedUser.Roles = [];
        }

        UserBan userBan = new UserBan
        {
            SteamId = command.SteamId,
            BannedOn = DateTimeOffset.UtcNow,
            Banner = banner,
            Reason = command.Reason,
        };

        _DbContext.Add(userBan);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}