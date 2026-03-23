using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.PublishedFileService;

public class QueryResult
{
    [JsonPropertyName("total")]
    public required int Total { get; set; }

    [JsonPropertyName("publishedfiledetails")]
    public required List<PublishedFileDetail> Files { get; set; }

    [JsonPropertyName("next_cursor")]
    public required string NextCursor { get; set; }
}