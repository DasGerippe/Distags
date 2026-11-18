using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.SteamRemoteStorage;

public class CollectionDetailsResult
{
    [JsonPropertyName("result")]
    public int ResultCode { get; set; }

    [JsonPropertyName("resultcount")]
    public int ResultCount { get; set; }

    [JsonPropertyName("collectiondetails")]
    public List<CollectionDetail> CollectionDetails { get; set; } = [];
}
