using Distags.Core.Commands;
using Distags.Core.Commands.ImportCreators;
using Distags.Core.Queries;
using Distags.Core.Queries.StalestCreators;
using Distags.Core.Queries.UnknownCreators;
using Microsoft.Extensions.Logging;
using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.SteamUser;

namespace SteamDataScraper.Creators;

public class CreatorScraper
{
    private readonly IQueryHandler<UnknownCreatorsQuery, List<long>> _UnknownCreatorsQueryHandler;
    private readonly IQueryHandler<StalestCreatorsQuery, List<long>> _StalestCreatorsQueryHandler;
    private readonly ICommandHandler<ImportCreatorsCommand> _ImportCreatorsCommandHandler;
    private readonly ISteamUser _SteamUser;
    private readonly ICreatorRefreshCountCalculator _RefreshCountCalculator;
    private readonly ICreatorRefreshDateManager _RefreshDateManager;

    private readonly string _SteamWebApiKey;

    private readonly ILogger<CreatorScraper> _Logger;

    public CreatorScraper(
        IQueryHandler<UnknownCreatorsQuery, List<long>> unknownCreatorsQueryHandler,
        IQueryHandler<StalestCreatorsQuery, List<long>> stalestCreatorsQueryHandler,
        ICommandHandler<ImportCreatorsCommand> importCreatorsCommandHandler,
        ISteamUser steamUser,
        ICreatorRefreshCountCalculator refreshCountCalculator,
        ICreatorRefreshDateManager refreshDateManager,
        SteamWebApiSettings steamWebApiSettings,
        ILogger<CreatorScraper> logger)
    {
        _UnknownCreatorsQueryHandler = unknownCreatorsQueryHandler;
        _StalestCreatorsQueryHandler = stalestCreatorsQueryHandler;
        _ImportCreatorsCommandHandler = importCreatorsCommandHandler;
        _SteamUser = steamUser;
        _RefreshCountCalculator = refreshCountCalculator;
        _RefreshDateManager = refreshDateManager;
        _SteamWebApiKey = steamWebApiSettings.ApiKey;

        _Logger = logger;
    }

    public async Task ScrapeLevelCreatorsAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Scraping level creators from Steam web API...");

        List<long> creatorIds = await GetCreatorIdsForScrapingAsync(cancellationToken).ConfigureAwait(false);
        List<ScrapedCreator> scrapedCreators = await ScrapeLevelCreatorsAsync(creatorIds, cancellationToken).ConfigureAwait(false);

        ImportCreatorsCommand importCreatorsCommand = new ImportCreatorsCommand { ScrapedCreators = scrapedCreators };
        await _ImportCreatorsCommandHandler.HandleAsync(importCreatorsCommand).ConfigureAwait(false);

        await _RefreshDateManager.SetRefreshDateAsync(DateTimeOffset.UtcNow, cancellationToken).ConfigureAwait(false);

        _Logger.LogDebug("Scraped level creators from Steam web API.");
    }

    private async Task<List<long>> GetCreatorIdsForScrapingAsync(CancellationToken cancellationToken)
    {
        _Logger.LogDebug("Determining IDs of creators to be scraped...");

        List<long> creatorIds = await _UnknownCreatorsQueryHandler.HandleAsync(new UnknownCreatorsQuery()).ConfigureAwait(false);

        int refreshCount = await _RefreshCountCalculator.CalculateCreatorRefreshCountAsync(cancellationToken).ConfigureAwait(false);
        if (refreshCount > 0)
        {
            StalestCreatorsQuery stalestCreatorsQuery = new StalestCreatorsQuery { Limit = refreshCount };
            List<long> stalestCreatorIds = await _StalestCreatorsQueryHandler.HandleAsync(stalestCreatorsQuery).ConfigureAwait(false);
            creatorIds.AddRange(stalestCreatorIds);
        }

        _Logger.LogDebug("Got {count} creator IDs for scraping.", creatorIds.Count);

        return creatorIds;
    }

    private async Task<List<ScrapedCreator>> ScrapeLevelCreatorsAsync(
        IEnumerable<long> creatorIds, CancellationToken cancellationToken = default)
    {
        GetPlayerSummariesParameters parameters = CreateParameters(creatorIds);
        IAsyncEnumerable<PlayerSummary> playerSummariesQuery = _SteamUser.GetPlayerSummariesAsync(parameters, cancellationToken);

        List<ScrapedCreator> scrapedCreators = [];

        await foreach (PlayerSummary playerSummary in playerSummariesQuery.ConfigureAwait(false))
        {
            scrapedCreators.AddRange(ToScrapedCreator(playerSummary));

            _Logger.LogDebug("Added scraped creator {id} to result.", playerSummary.SteamId);
        }

        return scrapedCreators;
    }

    private GetPlayerSummariesParameters CreateParameters(IEnumerable<long> creatorIds)
    {
        return new GetPlayerSummariesParameters
        {
            Key = _SteamWebApiKey,
            SteamIds = creatorIds,
        };
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