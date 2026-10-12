namespace Distags.Core.Exceptions;

public class UnknownUserSessionException : DistagsException
{
    public string SessionToken { get; }

    public UnknownUserSessionException(string sessionToken)
    {
        SessionToken = sessionToken;
    }
}
