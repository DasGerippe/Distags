using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.SteamUser;

namespace SteamDataScraper.Creators;

public class CreatorScraper
{
    private readonly LevelTagDbContext _LevelTagDbContext;
    private readonly ISteamUser _SteamUser;
    private readonly ICreatorRefreshCountCalculator _RefreshCountCalculator;
    private readonly ICreatorRefreshDateManager _RefreshDateManager;
    private readonly string _SteamWebApiKey;

    private readonly ILogger<CreatorScraper> _Logger;

    public CreatorScraper(
        LevelTagDbContext levelTagDbContext,
        ISteamUser steamUser,
        ICreatorRefreshCountCalculator refreshCountCalculator,
        ICreatorRefreshDateManager refreshDateManager,
        SteamWebApiSettings steamWebApiSettings,
        ILogger<CreatorScraper> logger)
    {
        _LevelTagDbContext = levelTagDbContext;
        _SteamUser = steamUser;
        _RefreshCountCalculator = refreshCountCalculator;
        _RefreshDateManager = refreshDateManager;
        _SteamWebApiKey = steamWebApiSettings.ApiKey;

        _Logger = logger;
    }

    public async Task ScrapeUnknownLevelCreatorsAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Scraping unknown level creators from Steam web API...");

        List<long> unknownCreatorIds = await _LevelTagDbContext.Set<Level>()
            .Where(level => level.Creator == null)
            .Select(level => level.CreatorId)
            .Distinct()
            .ToListAsync(cancellationToken).ConfigureAwait(false);

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

        int refreshCount = await _RefreshCountCalculator.CalculateCreatorRefreshCountAsync(cancellationToken).ConfigureAwait(false);
        if (refreshCount == 0)
        {
            _Logger.LogDebug("Cancelling scraping of outdated level creators.");
            return;
        }

        List<Creator> creators = await _LevelTagDbContext.Set<Creator>()
            .OrderBy(creator => creator.CachedOn)
            .Take(refreshCount)
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

        await _RefreshDateManager.SetRefreshDateAsync(DateTime.UtcNow, cancellationToken).ConfigureAwait(false);

        _Logger.LogDebug("Scraped outdated level creators from Steam web API.");
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