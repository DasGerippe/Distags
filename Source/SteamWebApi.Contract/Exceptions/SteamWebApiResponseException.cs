namespace SteamWebApi.Exceptions;

public class SteamWebApiResponseException : SteamWebApiException
{
    public SteamWebApiResponseException()
    {
    }

    public SteamWebApiResponseException(string? message) : base(message)
    {
    }

    public SteamWebApiResponseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}