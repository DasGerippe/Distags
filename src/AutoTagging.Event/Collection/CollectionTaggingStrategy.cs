using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.ChangeLevelTag;
using Distags.Core.Filtering.Levels;
using Distags.Core.Queries.AutoTaggerLevels;
using Distags.Core.Queries.Tags;
using Microsoft.Extensions.Logging;
using SteamWebApi.Dtos.SteamRemoteStorage;

namespace AutoTagging.Event.Collection;

public class CollectionTaggingStrategy : ITaggingStrategy
{
    private const string StateKey = "collections";

    private readonly AutoTaggerInfo _AutoTaggerInfo;
    private readonly StateStore _StateStore;
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly EventCollectionScraper _EventCollectionScraper;
    private readonly EventCollectionLevelsScraper _EventCollectionLevelsScraper;
    private readonly TagChanger _TagChanger;
    private readonly ILogger<CollectionTaggingStrategy> _Logger;

    public CollectionTaggingStrategy(
        AutoTaggerInfo autoTaggerInfo,
        StateStore stateStore,
        IRequestDispatcher requestDispatcher,
        EventCollectionScraper eventCollectionScraper,
        EventCollectionLevelsScraper eventCollectionLevelsScraper,
        TagChanger tagChanger,
        ILogger<CollectionTaggingStrategy> logger)
    {
        _AutoTaggerInfo = autoTaggerInfo;
        _StateStore = stateStore;
        _RequestDispatcher = requestDispatcher;
        _EventCollectionScraper = eventCollectionScraper;
        _EventCollectionLevelsScraper = eventCollectionLevelsScraper;
        _TagChanger = tagChanger;
        _Logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _Logger.LogDebug("Executing {strategy}...", nameof(CollectionTaggingStrategy));

        CollectionTaggingState state = await _StateStore.GetStateAsync<CollectionTaggingState>(StateKey).ConfigureAwait(false);

        List<EventCollection> eventCollections = await GetEventCollectionsForTagging(state, cancellationToken).ConfigureAwait(false);
        if (eventCollections.Count == 0)
            return;

        List<string> eventCollectionIds = eventCollections.Select(collection => collection.CollectionFileId).ToList();
        Dictionary<string, CollectionDetail> collectionDetails = await _EventCollectionLevelsScraper
            .ScrapeEventCollectionLevelsAsync(eventCollectionIds, cancellationToken).ConfigureAwait(false);

        try
        {
            foreach (EventCollection eventCollection in eventCollections)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (!collectionDetails.TryGetValue(eventCollection.CollectionFileId, out CollectionDetail? collectionDetail))
                {
                    _Logger.LogWarning("Could not scrape CollectionDetail for collection {collection}.",
                        eventCollection.CollectionFileId);
                    continue;
                }

                await TagEventCollectionLevels(eventCollection, collectionDetail).ConfigureAwait(false);

                state.CollectionDates[eventCollection.CollectionFileId] = eventCollection.CollectionUpdatedOn;
            }
        }
        finally
        {
            await _StateStore.SetStateAsync(StateKey, state).ConfigureAwait(false);
        }
    }

    private async Task<List<EventCollection>> GetEventCollectionsForTagging(
        CollectionTaggingState state, CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Determining event collections that need re-tagging...");

        List<TagDto> tags = await _RequestDispatcher.DispatchAsync(new TagsQuery()).ConfigureAwait(false);
        HashSet<string> existingTagNames = tags.Select(tag => tag.Name).ToHashSet();

        List<EventCollection> eventCollectionsForTagging = await _EventCollectionScraper
            .ScrapeEventCollectionsAsync(cancellationToken)
            .Where(eventCollection =>
                existingTagNames.Contains(eventCollection.EventTagName) &&
                eventCollection.CollectionUpdatedOn != state.CollectionDates.GetValueOrDefault(eventCollection.CollectionFileId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        _Logger.LogDebug("Got {count} new or updated event collections.", eventCollectionsForTagging.Count);

        return eventCollectionsForTagging;
    }

    private async Task TagEventCollectionLevels(EventCollection eventCollection, CollectionDetail collectionDetail)
    {
        _Logger.LogDebug("Tagging levels of event collection {collection}...", eventCollection.CollectionFileId);

        HashSet<string> eventLevels = await GetEventLevelsAsync(eventCollection.EventTagName).ConfigureAwait(false);

        List<string> untaggedLevelWorkshopIds = collectionDetail.Items
            .Select(collectionItem => collectionItem.PublishedFileId)
            .Except(eventLevels)
            .ToList();

        foreach (string untaggedLevelWorkshopId in untaggedLevelWorkshopIds)
        {
            ChangeWorkshopLevelTagCommand command = new ChangeWorkshopLevelTagCommand
            {
                LevelWorkshopId = long.Parse(untaggedLevelWorkshopId),
                TagName = eventCollection.EventTagName,
                Action = TagChangeAction.Add,
                AutoTaggerGuid = _AutoTaggerInfo.Guid,
            };
            await _TagChanger.TryChangeLevelTagAsync(command).ConfigureAwait(false);
        }
    }

    private async Task<HashSet<string>> GetEventLevelsAsync(string eventTagName)
    {
        AutoTaggerLevelsQuery eventLevelsQuery = new AutoTaggerLevelsQuery
        {
            FilterParameters = new LevelFilterParameters
            {
                Type = LevelType.Workshop,
                IncludeTags = [eventTagName],
            },
        };

        List<LevelDto> levels = await _RequestDispatcher.DispatchAsync(eventLevelsQuery).ConfigureAwait(false);
        HashSet<string> eventLevels = levels
            .OfType<WorkshopLevelDto>()
            .Select(level => level.WorkshopId.ToString())
            .ToHashSet();

        return eventLevels;
    }
}
