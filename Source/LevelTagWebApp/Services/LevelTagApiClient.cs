using HttpUtils;
using LevelTagApi.Dtos;
using System.Text.Json;

namespace LevelTagWebApp.Services;

public class LevelTagApiClient
{
    private readonly HttpClient _HttpClient;

    public LevelTagApiClient(HttpClient httpClient)
    {
        _HttpClient = httpClient;
    }

    public async Task<List<TagGroupDto>> GetTagTreeAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("api/tags/tree", cancellationToken).ConfigureAwait(false);
        List<TagGroupDto> tagTree = await GetContentAsync<List<TagGroupDto>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return tagTree;
    }

    public async Task<GetLevelsResponse> GetLevelsAsync(
        GetLevelsParameters parameters, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(parameters);
        string requestUri = $"api/levels?{query}";

        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        GetLevelsResponse levelsResponse = await GetContentAsync<GetLevelsResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return levelsResponse;
    }

    public async Task<GetLevelDto?> GetLevelAsync(long levelId, CancellationToken cancellationToken = default)
    {
        string requestUri = $"api/levels/{levelId}";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        GetLevelDto level = await GetContentAsync<GetLevelDto>(responseMessage, cancellationToken).ConfigureAwait(false);
        return level;
    }

    private async Task<TResponseContent> GetContentAsync<TResponseContent>(
        HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        responseMessage.EnsureSuccessStatusCode();

        Stream contentStream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
        TResponseContent? content = await JsonSerializer
            .DeserializeAsync<TResponseContent>(contentStream, JsonSerializerOptions.Web, cancellationToken)
            .ConfigureAwait(false);

        return content ?? throw new JsonException("Received invalid json.");
    }
}