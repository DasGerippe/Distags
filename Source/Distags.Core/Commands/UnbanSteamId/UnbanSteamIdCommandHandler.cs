using DataClasses;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.UnbanSteamId;

public class UnbanSteamIdCommandHandler : ICommandHandler<UnbanSteamIdCommand>
{
    private readonly DbContext _DbContext;

    public UnbanSteamIdCommandHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(UnbanSteamIdCommand command)
    {
        UserBan? userBan = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == command.SteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (userBan == null)
            throw new SteamIdNotBannedException();

        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.SteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        user?.IsBanned = false;

        _DbContext.Remove(userBan);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}