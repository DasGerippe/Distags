using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.SteamRemoteStorage;

public class CollectionItem
{

    [JsonPropertyName("publishedfileid")]
    public string PublishedFileId { get; set; } = string.Empty;

    [JsonPropertyName("sortorder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("filetype")]
    public int FileType { get; set; }
}
