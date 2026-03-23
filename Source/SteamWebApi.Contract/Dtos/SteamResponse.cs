using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos;

public class SteamResponse<TResponseContent>
{
    [JsonPropertyName("response")]
    public required TResponseContent Content { get; set; }
}