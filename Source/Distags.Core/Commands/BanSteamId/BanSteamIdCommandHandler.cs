using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.BanSteamId;

public class BanSteamIdCommandHandler : ICommandHandler<BanSteamIdCommand>
{
    private readonly DistagsDbContext _DbContext;

    public BanSteamIdCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(BanSteamIdCommand command)
    {
        if (command.SteamId == command.BannerSteamId)
            throw new CannotBanSelfException();

        User? banner = await GetUser(command.BannerSteamId).ConfigureAwait(false);
        if (banner == null)
            throw new UnknownUserException(command.BannerSteamId);

        User? bannedUser = await GetUser(command.SteamId).ConfigureAwait(false);

        if (bannedUser != null && !UserCanBanOtherUser(banner, bannedUser))
            throw new CannotBanOtherUserException();

        bool isSteamIdBanned = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == command.SteamId)
            .AnyAsync().ConfigureAwait(false);
        if (isSteamIdBanned)
            throw new SteamIdAlreadyBannedException();

        UserBan userBan = new UserBan
        {
            SteamId = command.SteamId,
            User = bannedUser,
            BannedOn = DateTimeOffset.UtcNow,
            Banner = banner,
            Reason = command.Reason,
        };

        if (bannedUser != null)
        {
            bannedUser.IsBanned = true;
            bannedUser.Roles = [];
        }

        _DbContext.Add(userBan);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private Task<User?> GetUser(long steamId)
    {
        return _DbContext.Set<User>()
            .Where(user => user.SteamId == steamId)
            .FirstOrDefaultAsync();
    }

    private bool UserCanBanOtherUser(User banner, User bannedUser)
    {
        if (bannedUser.Roles.Contains(Constants.UserRoles.Administrator))
            return false;

        if (bannedUser.Roles.Contains(Constants.UserRoles.Moderator))
            return banner.Roles.Contains(Constants.UserRoles.Administrator);

        return true;
    }
}