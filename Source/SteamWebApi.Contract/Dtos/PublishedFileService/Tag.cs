using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.PublishedFileService;

public class Tag
{
    [JsonPropertyName("tag")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;
}