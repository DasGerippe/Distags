using System.Net.Http.Headers;

namespace AutoTaggerRunner;

public class DistagsApiClient
{
    private readonly HttpClient _HttpClient;

    public DistagsApiClient(HttpClient httpClient)
    {
        _HttpClient = httpClient;
    }

    public async Task PutLevelTagAsync(long levelId, string tagName, string distagsApiKey, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        HttpRequestMessage requestMessage = CreateRequestMessage(HttpMethod.Put, requestUri, distagsApiKey);
        HttpResponseMessage responseMessage = await _HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task DeleteLevelTagAsync(long levelId, string tagName, string distagsApiKey, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        HttpRequestMessage requestMessage = CreateRequestMessage(HttpMethod.Delete, requestUri, distagsApiKey);
        HttpResponseMessage responseMessage = await _HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestUri, string distagsApiKey)
    {
        return new HttpRequestMessage(method, requestUri)
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("ApiKey", distagsApiKey),
            },
        };
    }
}