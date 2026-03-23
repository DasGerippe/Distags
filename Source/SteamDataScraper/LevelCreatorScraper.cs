using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.SteamUser;

namespace SteamDataScraper;

public class LevelCreatorScraper
{
    private readonly LevelTagDbContext _LevelTagDbContext;
    private readonly ISteamUser _SteamUser;
    private readonly string _SteamWebApiKey;

    private readonly ILogger<LevelCreatorScraper> _Logger;

    public LevelCreatorScraper(
        LevelTagDbContext levelTagDbContext,
        ISteamUser steamUser,
        [FromKeyedServices("SteamWebApiKey")] string steamWebApiKey,
        ILogger<LevelCreatorScraper> logger)
    {
        _LevelTagDbContext = levelTagDbContext;
        _SteamUser = steamUser;
        _SteamWebApiKey = steamWebApiKey;

        _Logger = logger;
    }

    public async Task ScrapeUnknownLevelCreatorsAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Scraping unknown level creators from Steam web API...");

        List<long> unknownCreatorIds = await _LevelTagDbContext.Set<Level>()
            .Where(level => level.Creator == null)
            .Select(level => level.CreatorId)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        _Logger.LogDebug("Found {count} unknown level creators.", unknownCreatorIds.Count);

        GetPlayerSummariesParameters parameters = CreateParameters(unknownCreatorIds);
        IAsyncEnumerable<PlayerSummary> playerSummariesQuery = _SteamUser.GetPlayerSummariesAsync(parameters, cancellationToken);

        await foreach (PlayerSummary playerSummary in playerSummariesQuery.ConfigureAwait(false))
        {
            Creator creator = new Creator();
            creator.Id = long.Parse(playerSummary.SteamId);
            SetCreatorProperties(creator, playerSummary);

            _LevelTagDbContext.Add(creator);
        }

        _Logger.LogDebug("Saving scraped level creators.");
        await _LevelTagDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _Logger.LogDebug("Scraped unknown level creators from Steam web API.");
    }

    public async Task ScrapeOutdatedLevelCreatorsAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Scraping outdated level creators from Steam web API...");

        int creatorRefreshCount = await CalculateCreatorRefreshCount(cancellationToken).ConfigureAwait(false);
        if (creatorRefreshCount == 0)
            return;

        List<Creator> creators = await _LevelTagDbContext.Set<Creator>()
            .OrderBy(creator => creator.CachedOn)
            .Take(creatorRefreshCount)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        GetPlayerSummariesParameters parameters = CreateParameters(creators.Select(creator => creator.Id));
        IAsyncEnumerable<PlayerSummary> playerSummariesQuery = _SteamUser.GetPlayerSummariesAsync(parameters, cancellationToken);

        await foreach (PlayerSummary playerSummary in playerSummariesQuery.ConfigureAwait(false))
        {
            long creatorId = long.Parse(playerSummary.SteamId);
            Creator? creator = await _LevelTagDbContext.FindAsync<Creator>([creatorId], cancellationToken).ConfigureAwait(false);
            if (creator != null)
                SetCreatorProperties(creator, playerSummary);
        }

        _Logger.LogDebug("Saving scraped level creators.");
        await _LevelTagDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _Logger.LogDebug("Scraped outdated level creators from Steam web API.");
    }

    private async Task<int> CalculateCreatorRefreshCount(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Calculating creator refresh count...");

        int creatorCount = await _LevelTagDbContext.Set<Creator>().CountAsync(cancellationToken).ConfigureAwait(false);
        if (creatorCount == 0)
        {
            _Logger.LogDebug("No creators exist. Cannot calculate refresh count.");
            return 0;
        }

        int refreshCycleInDays = 30;

        DateTime latestCacheDate = await _LevelTagDbContext.Set<Creator>()
            .MaxAsync(creator => creator.CachedOn, cancellationToken)
            .ConfigureAwait(false);

        double refreshPercentage = (DateTime.UtcNow - latestCacheDate).TotalDays / refreshCycleInDays;
        int refreshCount = (int)Math.Round(refreshPercentage * creatorCount);

        if (refreshCount is > 0 and < 10)
        {
            _Logger.LogDebug("Calculated refresh count is {refreshCount}. Not worth it. Return 0 instead.", refreshCount);
            return 0;
        }

        _Logger.LogDebug("Calculated refresh count is {refreshCount}.", refreshCount);
        return refreshCount;
    }

    private GetPlayerSummariesParameters CreateParameters(IEnumerable<long> creatorIds)
    {
        return new GetPlayerSummariesParameters
        {
            Key = _SteamWebApiKey,
            SteamIds = creatorIds,
        };
    }

    private void SetCreatorProperties(Creator creator, PlayerSummary playerSummary)
    {
        creator.Name = playerSummary.PersonaName;
        creator.AvatarHash = playerSummary.AvatarHash;
        creator.CachedOn = DateTime.UtcNow;
    }
}