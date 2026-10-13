namespace Distags.Core.Commands.CreateUserSession;

public class CreateUserSessionCommand : ICommand
{
    public long UserSteamId { get; set; }

    public string SessionToken { get; set; } = string.Empty;
}
