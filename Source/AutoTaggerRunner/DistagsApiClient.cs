using Distags.Contracts;
using System.Net.Http.Headers;

namespace AutoTaggerRunner;

public class DistagsApiClient
{
    private readonly HttpClient _HttpClient;

    public DistagsApiClient(HttpClient httpClient)
    {
        _HttpClient = httpClient;
    }

    public async Task PutLevelTagAsync(LevelDto level, string tagName, string distagsApiKey)
    {
        string requestUri = GetLevelTagUri(level, tagName);
        HttpRequestMessage requestMessage = CreateRequestMessage(HttpMethod.Put, requestUri, distagsApiKey);
        HttpResponseMessage responseMessage = await _HttpClient.SendAsync(requestMessage);
        await EnsureSuccessAsync(responseMessage);
    }

    public async Task DeleteLevelTagAsync(LevelDto level, string tagName, string distagsApiKey)
    {
        string requestUri = GetLevelTagUri(level, tagName);
        HttpRequestMessage requestMessage = CreateRequestMessage(HttpMethod.Put, requestUri, distagsApiKey);
        HttpResponseMessage responseMessage = await _HttpClient.SendAsync(requestMessage);
        await EnsureSuccessAsync(responseMessage);
    }

    private string GetLevelTagUri(LevelDto level, string tagName)
    {
        return $"levels/{GetLevelUriPart(level)}/tags/{tagName}";
    }

    private string GetLevelUriPart(LevelDto level)
    {
        return level switch
        {
            OfficialLevelDto officialLevel => $"official/{officialLevel.Slug}",
            WorkshopLevelDto workshopLevel => $"workshop/{workshopLevel.WorkshopId}",
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };
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

    private static async Task EnsureSuccessAsync(HttpResponseMessage responseMessage)
    {
        if (responseMessage.IsSuccessStatusCode)
            return;

        HttpRequestMessage requestMessage = responseMessage.RequestMessage!;
        string responseBody = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

        throw new HttpRequestException(
            $"""
             {requestMessage.Method} {requestMessage.RequestUri} failed with status code {(int)responseMessage.StatusCode}: {responseMessage.ReasonPhrase}.
             Response body: {(string.IsNullOrWhiteSpace(responseBody) ? "(empty)" : responseBody)}
             """,
            inner: null,
            statusCode: responseMessage.StatusCode);
    }
}
