namespace Distags.Core.Commands.CleanupUserSessions;

public class CleanupUserSessionsCommand : ICommand
{
    public TimeSpan SessionExpirationDelay { get; set; }
}