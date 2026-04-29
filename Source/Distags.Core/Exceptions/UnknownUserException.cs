namespace Distags.Core.Exceptions;

public class UnknownUserException : DistagsException
{
    public long SteamId { get; }

    public UnknownUserException(long steamId)
    {
        SteamId = steamId;
    }
}