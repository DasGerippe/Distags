namespace Distags.Core.Commands.DeleteUserSessions;

public class DeleteUserSessionsCommand : ICommand
{
    public long UserSteamId { get; set; }
}