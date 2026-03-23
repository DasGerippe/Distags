namespace SteamWebApi.Exceptions;

public class SteamWebApiException : Exception
{
    public SteamWebApiException()
    {
    }

    public SteamWebApiException(string? message) : base(message)
    {
    }

    public SteamWebApiException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}