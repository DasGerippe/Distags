using HttpUtils;
using Microsoft.Extensions.Logging;
using SteamWebApi.Dtos.PublishedFileService;
using System.Runtime.CompilerServices;

namespace SteamWebApi.ApiInterfaces;

internal class PublishedFileService : IPublishedFileService
{
    private const string QueryFilesEndpoint = "IPublishedFileService/QueryFiles/v1/";

    private readonly SteamWebApiClient _SteamWebApiClient;
    private readonly ILogger<PublishedFileService> _Logger;

    public PublishedFileService(SteamWebApiClient steamWebApiClient, ILogger<PublishedFileService> logger)
    {
        _SteamWebApiClient = steamWebApiClient;
        _Logger = logger;
    }

    public async IAsyncEnumerable<PublishedFileDetail> QueryFilesAsync(
        QueryFileParameters queryParameters, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? initialCursor = queryParameters.Cursor;

        while (true)
        {
            string query = QueryBuilder.BuildQueryString(queryParameters, new PropertyAttributeKeySelector());
            string relativeRequestUri = $"{QueryFilesEndpoint}?{query}";

            _Logger.LogDebug("Querying file batch with cursor \"{cursor}\".", queryParameters.Cursor);
            QueryResult queryResult = await _SteamWebApiClient
                .GetAsync<QueryResult>(relativeRequestUri, cancellationToken)
                .ConfigureAwait(false);
            _Logger.LogDebug("Received {count} file details.", queryResult.Files.Count);

            if (queryResult.Files.Count == 0)
                break;

            foreach (PublishedFileDetail file in queryResult.Files)
            {
                yield return file;
            }

            queryParameters.Cursor = queryResult.NextCursor;
        }

        queryParameters.Cursor = initialCursor;
    }
}
