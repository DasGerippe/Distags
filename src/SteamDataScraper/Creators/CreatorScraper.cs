using Distags.Core;
using Distags.Core.Commands.ImportCreators;
using Distags.Core.Queries.StalestCreators;
using Distags.Core.Queries.UnknownCreators;
using Microsoft.Extensions.Logging;
using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.SteamUser;

namespace SteamDataScraper.Creators;

public class CreatorScraper
{
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly ISteamUser _SteamUser;
    private readonly ICreatorRefreshCountCalculator _RefreshCountCalculator;
    private readonly ICreatorRefreshDateManager _RefreshDateManager;

    private readonly string _SteamWebApiKey;

    private readonly ILogger<CreatorScraper> _Logger;

    public CreatorScraper(
        IRequestDispatcher requestDispatcher,
        ISteamUser steamUser,
        ICreatorRefreshCountCalculator refreshCountCalculator,
        ICreatorRefreshDateManager refreshDateManager,
        SteamWebApiSettings steamWebApiSettings,
        ILogger<CreatorScraper> logger)
    {
        _RequestDispatcher = requestDispatcher;
        _SteamUser = steamUser;
        _RefreshCountCalculator = refreshCountCalculator;
        _RefreshDateManager = refreshDateManager;
        _SteamWebApiKey = steamWebApiSettings.ApiKey;

        _Logger = logger;
    }

    public async Task ScrapeLevelCreatorsAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Scraping level creators from Steam web API...");

        await ScrapeUnknownLevelCreatorsAsync(cancellationToken).ConfigureAwait(false);
        await ScrapeStaleLevelCreatorsAsync(cancellationToken).ConfigureAwait(false);

        _Logger.LogDebug("Scraped level creators from Steam web API.");
    }

    private async Task ScrapeUnknownLevelCreatorsAsync(CancellationToken cancellationToken = default)
    {
        List<long> unknownCreatorIds = await _RequestDispatcher.DispatchAsync(new UnknownCreatorsQuery()).ConfigureAwait(false);

        _Logger.LogDebug("Scraping {count} unknown level creators...", unknownCreatorIds.Count);

        List<ScrapedCreator> scrapedCreators = await ScrapeLevelCreatorsAsync(unknownCreatorIds, cancellationToken).ConfigureAwait(false);
        await ImportLevelCreatorsAsync(scrapedCreators, cancellationToken);
    }

    private async Task ScrapeStaleLevelCreatorsAsync(CancellationToken cancellationToken = default)
    {
        int refreshCount = await _RefreshCountCalculator.CalculateCreatorRefreshCountAsync(cancellationToken).ConfigureAwait(false);
        if (refreshCount <= 0)
            return;

        StalestCreatorsQuery stalestCreatorsQuery = new StalestCreatorsQuery { Limit = refreshCount };
        List<long> stalestCreatorIds = await _RequestDispatcher.DispatchAsync(stalestCreatorsQuery).ConfigureAwait(false);

        _Logger.LogDebug("Scraping {count} stalest level creators...", stalestCreatorIds.Count);

        List<ScrapedCreator> scrapedCreators = await ScrapeLevelCreatorsAsync(stalestCreatorIds, cancellationToken).ConfigureAwait(false);
        await ImportLevelCreatorsAsync(scrapedCreators, cancellationToken);

        await _RefreshDateManager.SetRefreshDateAsync(DateTimeOffset.UtcNow, cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<ScrapedCreator>> ScrapeLevelCreatorsAsync(
        IEnumerable<long> creatorIds, CancellationToken cancellationToken = default)
    {
        GetPlayerSummariesParameters parameters = new GetPlayerSummariesParameters
        {
            Key = _SteamWebApiKey,
            SteamIds = creatorIds,
        };
        IAsyncEnumerable<PlayerSummary> playerSummariesQuery = _SteamUser.GetPlayerSummariesAsync(parameters, cancellationToken);

        List<ScrapedCreator> scrapedCreators = [];

        await foreach (PlayerSummary playerSummary in playerSummariesQuery.ConfigureAwait(false))
        {
            scrapedCreators.AddRange(ToScrapedCreator(playerSummary));

            _Logger.LogDebug("Added scraped creator {id} to result.", playerSummary.SteamId);
        }

        return scrapedCreators;
    }

    private async Task ImportLevelCreatorsAsync(List<ScrapedCreator> scrapedCreators, CancellationToken cancellationToken = default)
    {
        ImportCreatorsCommand command = new ImportCreatorsCommand
        {
            ScrapedCreators = scrapedCreators,
        };
        await _RequestDispatcher.DispatchAsync(command);
    }

    private ScrapedCreator ToScrapedCreator(PlayerSummary playerSummary)
    {
        return new ScrapedCreator
        {
            Id = long.Parse(playerSummary.SteamId),
            Name = playerSummary.PersonaName,
            AvatarHash = playerSummary.AvatarHash,
        };
    }
}
