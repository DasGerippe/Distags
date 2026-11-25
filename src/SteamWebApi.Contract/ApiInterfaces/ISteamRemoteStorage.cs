using SteamWebApi.Dtos.SteamRemoteStorage;

namespace SteamWebApi.ApiInterfaces;

public interface ISteamRemoteStorage
{
    IAsyncEnumerable<CollectionDetail> GetCollectionDetailsAsync(
        GetCollectionDetailsParameters parameters, CancellationToken cancellationToken = default);
}
