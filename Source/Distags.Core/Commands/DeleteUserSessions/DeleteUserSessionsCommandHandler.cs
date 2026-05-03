using DataClasses;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.DeleteUserSessions;

public class DeleteUserSessionsCommandHandler : ICommandHandler<DeleteUserSessionsCommand>
{
    private readonly DistagsDbContext _DbContext;

    public DeleteUserSessionsCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(DeleteUserSessionsCommand command)
    {
        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.UserSteamId)
            .Include(user => user.Sessions)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (user == null)
            throw new UnknownUserException(command.UserSteamId);

        _DbContext.RemoveRange(user.Sessions);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}