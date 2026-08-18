namespace Distags.WebApp.Security;

public class SessionTokenProvider : ISessionTokenProvider
{
    private string? _SessionToken;

    public string? SessionToken
    {
        get => _SessionToken;
        set
        {
            if (_SessionToken == value)
                return;

            _SessionToken = value;
            SessionTokenChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? SessionTokenChanged;
}
