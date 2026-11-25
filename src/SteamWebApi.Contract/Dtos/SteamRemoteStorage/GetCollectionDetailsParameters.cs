using HttpUtils;

namespace SteamWebApi.Dtos.SteamRemoteStorage;

public class GetCollectionDetailsParameters
{
    [HttpParameter("collectioncount")]
    public int CollectionCount => CollectionIds.Count;

    [HttpParameter("publishedfileids")]
    public IReadOnlyCollection<string> CollectionIds { get; set; } = [];
}
