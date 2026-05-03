using DataClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Distags.Core.Commands.CleanupUserSessions;

public class CleanupUserSessionsCommandHandler : ICommandHandler<CleanupUserSessionsCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly ILogger<CleanupUserSessionsCommandHandler> _Logger;

    public CleanupUserSessionsCommandHandler(DistagsDbContext dbContext, ILogger<CleanupUserSessionsCommandHandler> logger)
    {
        _DbContext = dbContext;
        _Logger = logger;
    }

    public async Task HandleAsync(CleanupUserSessionsCommand command)
    {
        _Logger.LogDebug("Performing session cleanup.");

        DateTimeOffset cutOffDate = DateTimeOffset.UtcNow - command.SessionExpirationDelay;

        List<UserSession> deprecatedSessions = await _DbContext.Set<UserSession>()
            .Where(session => session.LastSeenOn < cutOffDate)
            .ToListAsync()
            .ConfigureAwait(false);

        _Logger.LogDebug("Found {sessionCount} deprecated sessions for cleanup.", deprecatedSessions.Count);

        _DbContext.RemoveRange(deprecatedSessions);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);

        _Logger.LogDebug("Deleted deprecated sessions.");
    }
}