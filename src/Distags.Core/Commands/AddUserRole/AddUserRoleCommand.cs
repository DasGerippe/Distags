namespace Distags.Core.Commands.AddUserRole;

public class AddUserRoleCommand : ICommand
{
    public long UserSteamId { get; set; }

    public string Role { get; set; } = string.Empty;
}
