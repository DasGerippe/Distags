using HttpUtils;
using SteamWebApi.Dtos.SteamRemoteStorage;
using System.Runtime.CompilerServices;

namespace SteamWebApi.ApiInterfaces;

public class SteamRemoteStorage : ISteamRemoteStorage
{
    private const string GetCollectionDetailsEndpoint = "ISteamRemoteStorage/GetCollectionDetails/v1/";

    private readonly SteamWebApiClient _SteamWebApiClient;

    public SteamRemoteStorage(SteamWebApiClient steamWebApiClient)
    {
        _SteamWebApiClient = steamWebApiClient;
    }

    public async IAsyncEnumerable<CollectionDetail> GetCollectionDetailsAsync(
        GetCollectionDetailsParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        HttpContent requestContent = HttpContentBuilder.BuildHttpContent(parameters, new PropertyAttributeKeySelector());
        CollectionDetailsResult collectionDetailsResult = await _SteamWebApiClient
            .PostAsync<CollectionDetailsResult>(GetCollectionDetailsEndpoint, requestContent, cancellationToken)
            .ConfigureAwait(false);

        foreach (CollectionDetail collectionDetail in collectionDetailsResult.CollectionDetails)
        {
            yield return collectionDetail;
        }
    }
}
