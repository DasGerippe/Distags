using HttpUtils;

namespace SteamWebApi.Dtos.SteamUser;

public class GetPlayerSummariesParameters
{
    [HttpParameter("key")]
    public string? Key { get; set; }

    [HttpParameter("steamids")]
    public IEnumerable<long> SteamIds { get; set; } = [];
}
