using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.AddUserRole;

public class AddUserRoleCommandHandler : ICommandHandler<AddUserRoleCommand>
{
    private readonly DistagsDbContext _DbContext;

    public AddUserRoleCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(AddUserRoleCommand command)
    {
        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.UserSteamId)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        if (user == null)
            throw new UnknownUserException(command.UserSteamId);

        if (!user.Roles.Contains(command.Role))
            user.Roles.Add(command.Role);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
