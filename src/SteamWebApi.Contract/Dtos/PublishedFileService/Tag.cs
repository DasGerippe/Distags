using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.PublishedFileService;

public class Tag
{
    [JsonPropertyName("tag")]
    public required string Name { get; set; }

    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}
