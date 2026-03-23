namespace SteamWebApi.Exceptions;

public class SteamWebApiRequestException : SteamWebApiException
{
    public SteamWebApiRequestException(HttpResponseMessage steamWebApiResponse)
    {
    }
}