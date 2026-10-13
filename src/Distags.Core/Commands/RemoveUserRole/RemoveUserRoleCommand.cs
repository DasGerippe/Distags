namespace Distags.Core.Commands.RemoveUserRole;

public class RemoveUserRoleCommand : ICommand
{
    public long UserSteamId { get; set; }

    public string Role { get; set; } = string.Empty;
}
