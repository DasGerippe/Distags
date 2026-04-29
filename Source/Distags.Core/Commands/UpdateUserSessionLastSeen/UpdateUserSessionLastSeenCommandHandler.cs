using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.UpdateUserSessionLastSeen;

public class UpdateUserSessionLastSeenCommandHandler : ICommandHandler<UpdateUserSessionLastSeenCommand>
{
    private readonly DbContext _DbContext;

    public UpdateUserSessionLastSeenCommandHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(UpdateUserSessionLastSeenCommand command)
    {
        UserSession? userSession = await _DbContext.Set<UserSession>()
            .Where(userSession => userSession.Token == command.SessionToken)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (userSession == null)
            return;

        userSession.LastSeenOn = DateTimeOffset.UtcNow;
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}