namespace Distags.Core.Commands.CreateOrUpdateUser;

public class CreateOrUpdateUserCommand : ICommand
{
    public long SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }
}
