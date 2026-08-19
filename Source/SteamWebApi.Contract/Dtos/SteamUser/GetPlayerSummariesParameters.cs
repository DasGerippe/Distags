using HttpUtils;

namespace SteamWebApi.Dtos.SteamUser;

public class GetPlayerSummariesParameters
{
    [QueryParameter("key")]
    public string? Key { get; set; }

    [QueryParameter("steamids")]
    public IEnumerable<long> SteamIds { get; set; } = [];
}
