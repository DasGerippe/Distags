using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.PublishedFileService;

namespace SteamDataScraper;

public class LevelScraper
{
    private const int DistanceAppId = 233610;

    private readonly LevelTagDbContext _LevelTagDbContext;
    private readonly IPublishedFileService _PublishedFileService;
    private readonly string _SteamApiKey;

    private readonly ILogger<LevelScraper> _Logger;

    public LevelScraper(
        LevelTagDbContext levelTagDbContext,
        IPublishedFileService publishedFileService,
        [FromKeyedServices("SteamApiKey")] string steamApiKey,
        ILogger<LevelScraper> logger)
    {
        _LevelTagDbContext = levelTagDbContext;
        _PublishedFileService = publishedFileService;
        _SteamApiKey = steamApiKey;

        _Logger = logger;
    }

    public async Task ScrapeLevelsAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Scraping levels from Steam web API...");

        DateTime? cutOffDate = await GetLatestLevelDate(cancellationToken).ConfigureAwait(false);

        await ScrapeNewLevelsAsync(cutOffDate, cancellationToken).ConfigureAwait(false);

        if (cutOffDate != null)
            await ScrapeUpdatedLevelsAsync(cutOffDate.Value, cancellationToken).ConfigureAwait(false);

        _Logger.LogDebug("Saving scraped levels.");
        await _LevelTagDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _Logger.LogDebug("Scraped levels from Steam web API.");
    }

    private async Task<DateTime?> GetLatestLevelDate(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Determining latest create or update date...");

        if (!await _LevelTagDbContext.Set<Level>().AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            _Logger.LogDebug("No levels found in database. Cannot determine latest date.");
            return null;
        }
        
        DateTime latestCreatedDate = await _LevelTagDbContext.Set<Level>()
            .MaxAsync(level => level.CreatedOn, cancellationToken).ConfigureAwait(false);
        DateTime latestUpdatedDate = await _LevelTagDbContext.Set<Level>()
            .MaxAsync(level => level.UpdatedOn, cancellationToken).ConfigureAwait(false);
        DateTime latestDate = latestCreatedDate > latestUpdatedDate ? latestCreatedDate : latestUpdatedDate;

        _Logger.LogDebug("Latest date is {latestDate}.", latestDate);
        return latestDate;
    }

    private async Task ScrapeNewLevelsAsync(DateTime? cutOffDate, CancellationToken cancellationToken = default)
    {
        int itemsPerPage = GetItemsPerPage(cutOffDate);
        QueryFileParameters queryParams = CreateQueryFileParameters(QueryType.RankedByPublicationDate, itemsPerPage);

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

            Level level = new Level();
            level.Id = long.Parse(publishedFile.PublishedFileId);
            SetLevelProperties(level, publishedFile);

            _LevelTagDbContext.Add(level);
        }
    }

    private async Task ScrapeUpdatedLevelsAsync(DateTime cutOffDate, CancellationToken cancellationToken = default)
    {
        int itemsPerPage = GetItemsPerPage(cutOffDate);
        QueryFileParameters queryParams = CreateQueryFileParameters(QueryType.RankedByLastUpdatedDate, itemsPerPage);

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

            long levelId = long.Parse(publishedFile.PublishedFileId);
            Level? level = await _LevelTagDbContext.FindAsync<Level>([levelId], cancellationToken).ConfigureAwait(false);
            if (level != null)
                SetLevelProperties(level, publishedFile);
        }
    }

    private static int GetItemsPerPage(DateTime? previousScrapeDate)
    {
        if (previousScrapeDate == null)
            return 100;

        TimeSpan? elapsed = DateTime.UtcNow - previousScrapeDate;

        if (elapsed < TimeSpan.FromDays(1))
            return 5;

        if (elapsed < TimeSpan.FromDays(7))
            return 10;

        if (elapsed < TimeSpan.FromDays(30))
            return 25;

        if (elapsed < TimeSpan.FromDays(180))
            return 50;

        return 100;
    }

    private QueryFileParameters CreateQueryFileParameters(QueryType queryType, int itemsPerPage)
    {
        return new QueryFileParameters
        {
            SteamApiKey = _SteamApiKey,
            AppId = DistanceAppId,
            QueryType = queryType,
            ItemsPerPage = itemsPerPage,
            ReturnMetadata = true,
        };
    }

    private void SetLevelProperties(Level level, PublishedFileDetail publishedFile)
    {
        level.Name = publishedFile.Title;
        level.CreatorId = long.Parse(publishedFile.CreatorId);
        level.FileName = publishedFile.Filename;
        level.FileUrl = publishedFile.FileUrl;
        level.IconUrl = publishedFile.PreviewUrl;
        level.CreatedOn = publishedFile.TimeCreated;
        level.UpdatedOn = publishedFile.TimeUpdated;
        level.WorkshopTags = publishedFile.Tags.Select(tag => tag.Name).ToArray();
    }
}