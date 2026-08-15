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
        await EnsureSuccessAsync(responseMessage, requestMessage, levelId, tagName, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteLevelTagAsync(long levelId, string tagName, string distagsApiKey, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        HttpRequestMessage requestMessage = CreateRequestMessage(HttpMethod.Delete, requestUri, distagsApiKey);
        HttpResponseMessage responseMessage = await _HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(responseMessage, requestMessage, levelId, tagName, cancellationToken).ConfigureAwait(false);
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
    
    private static async Task EnsureSuccessAsync(
        HttpResponseMessage responseMessage,
        HttpRequestMessage requestMessage,
        long levelId,
        string tagName,
        CancellationToken cancellationToken)
    {
        if (responseMessage.IsSuccessStatusCode)
            return;

        string responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        throw new HttpRequestException(
            $"""
            {requestMessage.Method} {requestMessage.RequestUri} failed with status code {(int)responseMessage.StatusCode}: {responseMessage.ReasonPhrase}
            {responseMessage.StatusCode} for level {levelId}, tag '{tagName}'. 
            Response body: {(string.IsNullOrWhiteSpace(responseBody) ? "(empty)" : responseBody)}
            """, 
            inner: null,
            statusCode: responseMessage.StatusCode);
    }
}