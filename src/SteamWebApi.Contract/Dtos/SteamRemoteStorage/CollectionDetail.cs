using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.SteamRemoteStorage;

public class CollectionDetail
{
    [JsonPropertyName("publishedfileid")]
    public string CollectionId { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public int ResultCode { get; set; }

    [JsonPropertyName("children")]
    public List<CollectionItem> Items { get; set; } = [];
}
