using Distags.Core;
using Distags.Core.Commands.ImportWorkshopLevels;
using Distags.Core.Queries.LatestWorkshopLevelDate;
using Microsoft.Extensions.Logging;
using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.PublishedFileService;

namespace SteamDataScraper.Levels;

public class LevelScraper
{
    private const int DistanceAppId = 233610;

    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly IPublishedFileService _PublishedFileService;

    private readonly string _SteamWebApiKey;

    private readonly ILogger<LevelScraper> _Logger;

    public LevelScraper(
        IRequestDispatcher requestDispatcher,
        IPublishedFileService publishedFileService,
        SteamWebApiSettings steamWebApiSettings,
        ILogger<LevelScraper> logger)
    {
        _RequestDispatcher = requestDispatcher;
        _PublishedFileService = publishedFileService;
        _SteamWebApiKey = steamWebApiSettings.ApiKey;

        _Logger = logger;
    }

    public async Task ScrapeLevelsAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Scraping levels from Steam web API...");

        DateTimeOffset? cutOffDate = await _RequestDispatcher.DispatchAsync(new LatestWorkshopLevelDateQuery()).ConfigureAwait(false);

        List<ScrapedWorkshopLevel> scrapedLevels = await ScrapeNewLevelsAsync(cutOffDate, cancellationToken).ConfigureAwait(false);

        if (cutOffDate != null)
            scrapedLevels.AddRange(await ScrapeUpdatedLevelsAsync(cutOffDate.Value, cancellationToken).ConfigureAwait(false));

        ImportWorkshopLevelsCommand importLevelsCommand = new ImportWorkshopLevelsCommand { ScrapedWorkshopLevels = scrapedLevels };
        await _RequestDispatcher.DispatchAsync(importLevelsCommand).ConfigureAwait(false);

        _Logger.LogDebug("Scraped levels from Steam web API.");
    }

    private async Task<List<ScrapedWorkshopLevel>> ScrapeNewLevelsAsync(
        DateTimeOffset? cutOffDate, CancellationToken cancellationToken = default)
    {
        int itemsPerPage = GetItemsPerPage(cutOffDate);
        QueryFileParameters queryParams = CreateQueryFileParameters(QueryType.RankedByPublicationDate, itemsPerPage);
        List<ScrapedWorkshopLevel> scrapedLevels = [];

        _Logger.LogDebug("Scraping new levels...");

        IAsyncEnumerable<PublishedFileDetail> publishedFileQuery = _PublishedFileService.QueryFilesAsync(queryParams, cancellationToken);
        await foreach (PublishedFileDetail publishedFile in publishedFileQuery.ConfigureAwait(false))
        {
            if (cutOffDate != null && publishedFile.TimeCreated <= cutOffDate)
            {
                _Logger.LogDebug("Scraped level {id} was created before cut-off date. Breaking out of query loop.",
                    publishedFile.PublishedFileId);
                break;
            }

            scrapedLevels.Add(ToScrapedLevel(publishedFile));

            _Logger.LogDebug("Added scraped level {id} to result.", publishedFile.PublishedFileId);
        }

        return scrapedLevels;
    }

    private async Task<List<ScrapedWorkshopLevel>> ScrapeUpdatedLevelsAsync(
        DateTimeOffset cutOffDate, CancellationToken cancellationToken = default)
    {
        int itemsPerPage = GetItemsPerPage(cutOffDate);
        QueryFileParameters queryParams = CreateQueryFileParameters(QueryType.RankedByLastUpdatedDate, itemsPerPage);
        List<ScrapedWorkshopLevel> scrapedLevels = [];

        _Logger.LogDebug("Scraping updated levels...");

        IAsyncEnumerable<PublishedFileDetail> publishedFileQuery = _PublishedFileService.QueryFilesAsync(queryParams, cancellationToken);
        await foreach (PublishedFileDetail publishedFile in publishedFileQuery.ConfigureAwait(false))
        {
            if (publishedFile.TimeUpdated <= cutOffDate)
            {
                _Logger.LogDebug("Scraped level {id} was updated before cut-off date. Breaking out of query loop.",
                    publishedFile.PublishedFileId);
                break;
            }

            if (publishedFile.TimeCreated >= cutOffDate)
            {
                _Logger.LogDebug("Skipping updated level {id} because it was already processed when scraping new levels.",
                    publishedFile.PublishedFileId);
                continue;
            }

            scrapedLevels.Add(ToScrapedLevel(publishedFile));

            _Logger.LogDebug("Added scraped level {id} to result.", publishedFile.PublishedFileId);
        }

        return scrapedLevels;
    }

    private static int GetItemsPerPage(DateTimeOffset? previousScrapeDate)
    {
        if (previousScrapeDate == null)
            return 100;

        TimeSpan? elapsed = DateTimeOffset.UtcNow - previousScrapeDate;

        if (elapsed < TimeSpan.FromDays(1))
            return 5;

        if (elapsed < TimeSpan.FromDays(7))
            return 10;

        if (elapsed < TimeSpan.FromDays(30))
            return 25;

        if (elapsed < TimeSpan.FromDays(90))
            return 50;

        return 100;
    }

    private QueryFileParameters CreateQueryFileParameters(QueryType queryType, int itemsPerPage)
    {
        return new QueryFileParameters
        {
            Key = _SteamWebApiKey,
            AppId = DistanceAppId,
            QueryType = queryType,
            ItemsPerPage = itemsPerPage,
            ReturnMetadata = true,
        };
    }

    private ScrapedWorkshopLevel ToScrapedLevel(PublishedFileDetail publishedFile)
    {
        return new ScrapedWorkshopLevel
        {
            WorkshopId = long.Parse(publishedFile.PublishedFileId),
            Name = publishedFile.Title,
            CreatorId = long.Parse(publishedFile.CreatorId),
            FileName = publishedFile.Filename,
            FileUrl = publishedFile.FileUrl,
            ImageUrl = publishedFile.PreviewUrl,
            CreatedOn = publishedFile.TimeCreated,
            UpdatedOn = publishedFile.TimeUpdated,
            WorkshopTags = publishedFile.Tags.Select(tag => tag.Name).ToList(),
        };
    }
}
