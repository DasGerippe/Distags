namespace AutoTaggerRunner;

public class LevelTagApiClient
{
    private readonly HttpClient _HttpClient;

    public LevelTagApiClient(HttpClient httpClient)
    {
        _HttpClient = httpClient;
    }

    public async Task PostLevelTagAsync(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task DeleteLevelTagAsync(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        HttpResponseMessage responseMessage = await _HttpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }
}