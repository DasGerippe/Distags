using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.SteamRemoteStorage;

namespace AutoTagging.Event.Collection;

public class EventCollectionLevelsScraper
{
    private readonly ISteamRemoteStorage _SteamRemoteStorage;

    public EventCollectionLevelsScraper(ISteamRemoteStorage steamRemoteStorage)
    {
        _SteamRemoteStorage = steamRemoteStorage;
    }

    public async Task<Dictionary<string, CollectionDetail>> ScrapeEventCollectionLevelsAsync(
        IReadOnlyCollection<string> collectionFileIds, CancellationToken cancellationToken = default)
    {
        GetCollectionDetailsParameters parameters = new GetCollectionDetailsParameters
        {
            CollectionIds = collectionFileIds,
        };
        return await _SteamRemoteStorage.GetCollectionDetailsAsync(parameters, cancellationToken)
            .ToDictionaryAsync(collection => collection.CollectionId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
