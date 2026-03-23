using Microsoft.Extensions.Logging;
using SteamWebApi.Dtos.SteamUser;
using System.Runtime.CompilerServices;

namespace SteamWebApi.ApiInterfaces;

public class SteamUser : ISteamUser
{
    private const string GetPlayerSummariesEndpoint = "ISteamUser/GetPlayerSummaries/v2/";
    private const int MaxSteamUserIdCount = 100;

    private readonly SteamWebApiClient _SteamWebApiClient;
    private readonly ILogger<SteamUser> _Logger;

    public SteamUser(SteamWebApiClient steamWebApiClient, ILogger<SteamUser> logger)
    {
        _SteamWebApiClient = steamWebApiClient;
        _Logger = logger;
    }

    public async IAsyncEnumerable<PlayerSummary> GetPlayerSummariesAsync(
        GetPlayerSummariesParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IEnumerable<long[]> steamUserIdChunks = parameters.SteamIds.Chunk(MaxSteamUserIdCount);
        QueryBuilder queryBuilder = new QueryBuilder();

        GetPlayerSummariesParameters chunkParameters = new GetPlayerSummariesParameters()
        {
            SteamApiKey = parameters.SteamApiKey,
        };

        foreach (long[] steamUserIdChunk in steamUserIdChunks)
        {
            if (steamUserIdChunk.Length == 0)
                continue;

            chunkParameters.SteamIds = steamUserIdChunk;

            string query = queryBuilder.BuildQueryString(chunkParameters);
            string relativeRequestUri = $"{GetPlayerSummariesEndpoint}?{query}";

            _Logger.LogDebug("Getting next batch of player summaries.");
            PlayerSummaries playerSummaries = await _SteamWebApiClient
                .GetAsync<PlayerSummaries>(relativeRequestUri, cancellationToken)
                .ConfigureAwait(false);

            foreach (PlayerSummary playerSummary in playerSummaries.Players)
            {
                yield return playerSummary;
            }
        }
    }
}