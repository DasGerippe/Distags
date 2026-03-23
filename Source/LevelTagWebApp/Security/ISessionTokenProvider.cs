namespace LevelTagWebApp.Security;

public interface ISessionTokenProvider
{
    string? SessionToken { get; set; }

    event EventHandler SessionTokenChanged;
}