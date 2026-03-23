namespace SteamWebApi.Dtos.SteamUser;

public class GetPlayerSummariesParameters
{
    [QueryParameter("key")]
    public string? SteamApiKey { get; set; }

    [QueryParameter("steamids")]
    public IEnumerable<long> SteamIds { get; set; } = [];
}