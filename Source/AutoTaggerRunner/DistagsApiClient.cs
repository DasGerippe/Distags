namespace AutoTaggerRunner;

public class DistagsApiClient
{
    private readonly HttpClient _HttpClient;

    public DistagsApiClient(HttpClient httpClient)
    {
        _HttpClient = httpClient;
    }

    public async Task PutLevelTagAsync(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        HttpResponseMessage responseMessage = await _HttpClient.PutAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task DeleteLevelTagAsync(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        HttpResponseMessage responseMessage = await _HttpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }
}