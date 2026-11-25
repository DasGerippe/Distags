using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.PublishedFileService;
using System.Runtime.CompilerServices;

namespace AutoTagging.Event.Collection;

public class EventCollectionScraper
{
    private const int DistanceAppId = 233610;

    private readonly IPublishedFileService _PublishedFileService;
    private readonly string _SteamWebApiKey;

    private readonly List<CollectionNameEventSource> _CollectionNameEventSources =
    [
        new CollectionNameEventSource("DAC", @"DAC (\d{4})", "dac-{0}"),
        new CollectionNameEventSource("Dread AND Fest", @"Dread Fest (\d{4})", "dread-{0}"),
        new CollectionNameEventSource("Edit AND Clock", @"Edit the Clock! Round (\d+)", "etc-{0}"),
        new CollectionNameEventSource("Prompt AND Month", @"Prompt of the Month #(\d+)", "potm-{0}"),
    ];

    public EventCollectionScraper(
        IPublishedFileService publishedFileService,
        SteamWebApiSettings steamWebApiSettings)
    {
        _PublishedFileService = publishedFileService;
        _SteamWebApiKey = steamWebApiSettings.ApiKey;
    }

    public async IAsyncEnumerable<EventCollection> ScrapeEventCollectionsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string searchText = string.Join(" OR ", _CollectionNameEventSources
            .Select(source => $"({source.CollectionSearchText})"));

        QueryFileParameters queryParams = new QueryFileParameters
        {
            Key = _SteamWebApiKey,
            AppId = DistanceAppId,
            FileType = FileType.Collections,
            QueryType = QueryType.RankedByPublicationDate,
            SearchText = searchText,
            SearchTextTarget = SearchTextTarget.Title,
            ItemsPerPage = 100,
            ReturnMetadata = true,
        };

        IAsyncEnumerable<PublishedFileDetail> collectionQuery = _PublishedFileService.QueryFilesAsync(queryParams, cancellationToken);
        await foreach (PublishedFileDetail collection in collectionQuery.ConfigureAwait(false))
        {
            foreach (CollectionNameEventSource source in _CollectionNameEventSources)
            {
                string? eventTagName = source.GetEventTagName(collection.Title);
                if (eventTagName == null)
                    continue;

                yield return new EventCollection
                {
                    CollectionFileId = collection.PublishedFileId,
                    CollectionUpdatedOn = collection.TimeUpdated,
                    EventTagName = eventTagName,
                };
                break;
            }
        }
    }
}
