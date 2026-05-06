using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.DeleteUserSession;

public class DeleteUserSessionCommandHandler : ICommandHandler<DeleteUserSessionCommand>
{
    private readonly DistagsDbContext _DbContext;

    public DeleteUserSessionCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(DeleteUserSessionCommand command)
    {
        UserSession? userSession = await _DbContext.Set<UserSession>()
            .Where(userSession => userSession.Token == command.SessionToken)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (userSession == null)
            throw new UnknownUserSessionException(command.SessionToken);

        _DbContext.Remove(userSession);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}