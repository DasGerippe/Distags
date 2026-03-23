namespace LevelTagWebApp.Security;

public interface IUserInfoProvider
{
    long? SteamId { get; }

    string? SteamName { get; }

    string? SteamAvatarHash { get; }

    IEnumerable<string> Roles { get; }

    event EventHandler? UserInfoChanged;
}