namespace Distags.WebApp.Security;

public interface ISessionTokenProvider
{
    string? SessionToken { get; set; }

    event EventHandler SessionTokenChanged;
}