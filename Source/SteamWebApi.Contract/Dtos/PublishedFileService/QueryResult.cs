using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.PublishedFileService;

public class QueryResult
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("publishedfiledetails")]
    public List<PublishedFileDetail> Files { get; set; } = [];

    [JsonPropertyName("next_cursor")]
    public string NextCursor { get; set; } = string.Empty;
}