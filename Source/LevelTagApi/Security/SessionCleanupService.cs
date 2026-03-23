using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LevelTagApi.Security;

public class SessionCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _ScopeFactory;
    private readonly ILogger<SessionCleanupService> _Logger;

    private readonly TimeSpan _SessionExpirationDelay;

    public SessionCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptions<LevelTagApiSettings> options,
        ILogger<SessionCleanupService> logger)
    {
        _ScopeFactory = scopeFactory;
        _Logger = logger;

        _SessionExpirationDelay = options.Value.SessionExpirationDelay;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DeleteDeprecatedSessionsAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _Logger.LogDebug($"Execution of {nameof(SessionCleanupService)} was cancelled.");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "An error occurred during session cleanup.");
            }
        }
    }

    private async Task DeleteDeprecatedSessionsAsync(CancellationToken stoppingToken)
    {
        _Logger.LogDebug("Performing session cleanup.");

        using IServiceScope scope = _ScopeFactory.CreateScope();
        LevelTagDbContext dbContext = scope.ServiceProvider.GetRequiredService<LevelTagDbContext>();

        DateTime cutOffDate = DateTime.UtcNow - _SessionExpirationDelay;

        List<UserSession> deprecatedSessions = await dbContext.Set<UserSession>()
            .Where(session => session.LastSeenOn < cutOffDate)
            .ToListAsync(stoppingToken);

        _Logger.LogDebug("Found {sessionCount} deprecated sessions for cleanup.", deprecatedSessions.Count);

        if (deprecatedSessions.Count > 0)
        {
            foreach (UserSession deprecatedSession in deprecatedSessions)
            {
                dbContext.Remove(deprecatedSession);
            }
            await dbContext.SaveChangesAsync(stoppingToken);

            _Logger.LogDebug("Deleted deprecated sessions.");
        }
    }
}