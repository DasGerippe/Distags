using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.SteamUser;

public class PlayerSummaries
{
    [JsonPropertyName("players")]
    public required List<PlayerSummary> Players { get; set; }
}
