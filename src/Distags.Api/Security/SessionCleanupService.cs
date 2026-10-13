using Distags.Core;
using Distags.Core.Commands;
using Distags.Core.Commands.CleanupUserSessions;
using Microsoft.Extensions.Options;

namespace Distags.Api.Security;

public class SessionCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _ScopeFactory;
    private readonly ILogger<SessionCleanupService> _Logger;

    private readonly TimeSpan _SessionExpirationDelay;

    public SessionCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptions<DistagsApiSettings> options,
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
                try
                {
                    await DeleteDeprecatedSessionsAsync();
                }
                catch (Exception ex) when (ex is not TaskCanceledException)
                {
                    _Logger.LogError(ex, "An error occurred during session cleanup.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _Logger.LogDebug($"Execution of {nameof(SessionCleanupService)} was cancelled.");
            }
        }
    }

    private async Task DeleteDeprecatedSessionsAsync()
    {
        using IServiceScope scope = _ScopeFactory.CreateScope();
        IRequestDispatcher requestDispatcher = scope.ServiceProvider.GetRequiredService<IRequestDispatcher>();

        CleanupUserSessionsCommand command = new CleanupUserSessionsCommand
        {
            SessionExpirationDelay = _SessionExpirationDelay,
        };
        await requestDispatcher.DispatchAsync(command);
    }
}
