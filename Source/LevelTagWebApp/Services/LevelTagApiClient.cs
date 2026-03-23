using HttpUtils;
using LevelTagApi.Dtos;
using LevelTagWebApp.Security;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LevelTagWebApp.Services;

public class LevelTagApiClient : IDisposable
{
    private readonly HttpClient _HttpClient;
    private readonly ISessionTokenProvider _SessionTokenProvider;

    public Uri LevelHubEndpoint { get; }

    public Uri ContributionHubEndpoint { get; }

    public LevelTagApiClient(HttpClient httpClient, ISessionTokenProvider sessionTokenProvider)
    {
        _HttpClient = httpClient;
        _SessionTokenProvider = sessionTokenProvider;

        if (_HttpClient.BaseAddress == null)
            throw new ArgumentException("BaseAddress of HttpClient not set.", nameof(httpClient));

        LevelHubEndpoint = new Uri(_HttpClient.BaseAddress, "hubs/levels");
        ContributionHubEndpoint = new Uri(_HttpClient.BaseAddress, "hubs/contributions");

        UpdateAuthorizationRequestHeader();
        _SessionTokenProvider.SessionTokenChanged += OnSessionTokenChanged;
    }

    public void Dispose()
    {
        _SessionTokenProvider.SessionTokenChanged -= OnSessionTokenChanged;
    }

    private void OnSessionTokenChanged(object? sender, EventArgs args)
    {
        UpdateAuthorizationRequestHeader();
    }

    private void UpdateAuthorizationRequestHeader()
    {
        string? sessionToken = _SessionTokenProvider.SessionToken;
        _HttpClient.DefaultRequestHeaders.Authorization = sessionToken != null
            ? new AuthenticationHeaderValue(LevelTagApiConstants.AuthorizationHeaderSchemes.SessionToken, sessionToken)
            : null;
    }

    public async Task<List<TagGroupDto>> GetTagTreeAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("api/tags/tree", cancellationToken).ConfigureAwait(false);
        List<TagGroupDto> tagTree = await GetContentAsync<List<TagGroupDto>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return tagTree;
    }

    public async Task<BrowseLevelsResponse> BrowseLevelsAsync(
        BrowseLevelsParameters parameters, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(parameters);
        string requestUri = $"api/levels?{query}";

        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        BrowseLevelsResponse levelsResponse = await GetContentAsync<BrowseLevelsResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return levelsResponse;
    }

    public async Task<LevelDto?> GetLevelAsync(long levelId, CancellationToken cancellationToken = default)
    {
        string requestUri = $"api/levels/{levelId}";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        LevelDto level = await GetContentAsync<LevelDto>(responseMessage, cancellationToken).ConfigureAwait(false);
        return level;
    }

    public async Task PostLevelTag(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"api/levels/{levelId}/tags/{tagName}";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task DeleteLevelTag(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"api/levels/{levelId}/tags/{tagName}";
        HttpResponseMessage responseMessage = await _HttpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public string GetSteamLoginEndpoint(string? returnUrl = null)
    {
        if (_HttpClient.BaseAddress == null)
            throw new InvalidOperationException("BaseAddress of HttpClient not set.");

        UriBuilder uriBuilder = new UriBuilder(_HttpClient.BaseAddress);
        uriBuilder.Path = "api/authentication/steam-login";
        if (!string.IsNullOrEmpty(returnUrl))
            uriBuilder.Query = $"returnUrl={returnUrl}";

        return uriBuilder.ToString();
    }

    public async Task<SessionResponse> PostRedeemTicketAsync(string ticket, CancellationToken cancellationToken = default)
    {
        string requestUri = $"api/authentication/redeem-ticket?ticket={Uri.EscapeDataString(ticket)}";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        SessionResponse sessionResponse = await GetContentAsync<SessionResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return sessionResponse;
    }

    public async Task ValidateSession(CancellationToken cancellationToken = default)
    {
        string requestUri = "api/authentication/validate-session";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task PostLogout(CancellationToken cancellationToken = default)
    {
        string requestUri = "api/authentication/logout";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task PostLogoutAll(CancellationToken cancellationToken = default)
    {
        string requestUri = "api/authentication/logout-all";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<UserDto> GetMe(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("api/users/me", cancellationToken).ConfigureAwait(false);
        UserDto user = await GetContentAsync<UserDto>(responseMessage, cancellationToken).ConfigureAwait(false);
        return user;
    }

    public async Task<GetContributionsResponse> GetContributionsAsync(
        GetContributionsParameters parameters, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(parameters);
        string requestUri = $"api/contributions?{query}";

        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        GetContributionsResponse response =
            await GetContentAsync<GetContributionsResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<HttpResponseMessage> PostUserBan(
        CreateUserBanDto createUserBanDto, CancellationToken cancellationToken = default)
    {
        return await _HttpClient.PostAsJsonAsync("api/userbans", createUserBanDto, cancellationToken).ConfigureAwait(false);
    }

    public async Task<RevertContributionsResponse> RevertContributionsAsync(
        RevertContributionsParameters parameters, CancellationToken cancellationToken = default)
    {
        string requestUri = "api/contributions/revert";

        HttpResponseMessage responseMessage =
            await _HttpClient.PostAsJsonAsync(requestUri, parameters, cancellationToken).ConfigureAwait(false);
        RevertContributionsResponse response =
            await GetContentAsync<RevertContributionsResponse>(responseMessage, cancellationToken);

        return response;
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