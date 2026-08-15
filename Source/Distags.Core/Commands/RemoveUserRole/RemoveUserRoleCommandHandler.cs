using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.RemoveUserRole;

public class RemoveUserRoleCommandHandler : ICommandHandler<RemoveUserRoleCommand>
{
    private readonly DistagsDbContext _DbContext;

    public RemoveUserRoleCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(RemoveUserRoleCommand command)
    {
        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.UserSteamId)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        if (user == null)
            throw new UnknownUserException(command.UserSteamId);

        user.Roles.Remove(command.Role);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
