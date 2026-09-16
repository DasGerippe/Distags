using Distags.Api.Contracts;
using Distags.Api.Contracts.Authentication;
using Distags.Api.Contracts.Configuration;
using Distags.Api.Contracts.Contributions;
using Distags.Api.Contracts.DisplayGroups;
using Distags.Api.Contracts.Guidelines;
using Distags.Api.Contracts.Levels;
using Distags.Api.Contracts.LogicGroups;
using Distags.Api.Contracts.Tags;
using Distags.Api.Contracts.UserBans;
using Distags.Api.Contracts.Users;
using Distags.WebApp.Security;
using HttpUtils;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Distags.WebApp.Services;

public class DistagsApiClient : IDisposable
{
    private readonly HttpClient _HttpClient;
    private readonly JsonSerializerOptions _JsonOptions;
    private readonly ISessionTokenProvider _SessionTokenProvider;

    public Uri LevelHubEndpoint { get; }

    public Uri ContributionHubEndpoint { get; }

    public DistagsApiClient(HttpClient httpClient, ISessionTokenProvider sessionTokenProvider)
    {
        _HttpClient = httpClient;
        _SessionTokenProvider = sessionTokenProvider;

        _JsonOptions = new JsonSerializerOptions(JsonSerializerOptions.Web);
        _JsonOptions.Converters.Add(new PatchValueConverterFactory());
        _JsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

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
            ? new AuthenticationHeaderValue(DistagsApiConstants.AuthorizationHeaderSchemes.SessionToken, sessionToken)
            : null;
    }

    public async Task<List<DisplayGroupResponse>> GetDisplayGroupsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("displaygroups", cancellationToken).ConfigureAwait(false);
        List<DisplayGroupResponse> displayGroups = await GetContentAsync<List<DisplayGroupResponse>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return displayGroups;
    }

    public async Task<List<LogicGroupResponse>> GetLogicGroupsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("logicgroups", cancellationToken).ConfigureAwait(false);
        List<LogicGroupResponse> logicGroups = await GetContentAsync<List<LogicGroupResponse>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return logicGroups;
    }

    public async Task<List<TagResponse>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("tags", cancellationToken).ConfigureAwait(false);
        List<TagResponse> tags = await GetContentAsync<List<TagResponse>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return tags;
    }

    public async Task<LevelsPageResponse> BrowseLevelsAsync(
        BrowseLevelsRequest request, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(request);
        string requestUri = $"levels?{query}";

        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        LevelsPageResponse levelsPage = await GetContentAsync<LevelsPageResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return levelsPage;
    }

    public async Task<OfficialLevelResponse> GetOfficialLevelAsync(string levelSlug, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/official/{levelSlug}";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        OfficialLevelResponse level = await GetContentAsync<OfficialLevelResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return level;
    }

    public async Task<WorkshopLevelResponse> GetWorkshopLevelAsync(long levelWorkshopId, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/workshop/{levelWorkshopId}";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        WorkshopLevelResponse level = await GetContentAsync<WorkshopLevelResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return level;
    }

    public Task PutLevelTagAsync(LevelResponse level, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = GetLevelTagUri(level, tagName);
        return _HttpClient.PutAsync(requestUri, null, cancellationToken);
    }

    public Task DeleteLevelTagAsync(LevelResponse level, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = GetLevelTagUri(level, tagName);
        return _HttpClient.DeleteAsync(requestUri, cancellationToken);
    }

    private string GetLevelTagUri(LevelResponse level, string tagName)
    {
        return $"levels/{level.Key}/tags/{tagName}";
    }

    public string GetSteamLoginEndpoint(string? returnUrl = null)
    {
        if (_HttpClient.BaseAddress == null)
            throw new InvalidOperationException("BaseAddress of HttpClient not set.");

        UriBuilder uriBuilder = new UriBuilder(_HttpClient.BaseAddress);
        uriBuilder.Path = "authentication/steam-login";
        if (!string.IsNullOrEmpty(returnUrl))
            uriBuilder.Query = $"returnUrl={returnUrl}";

        return uriBuilder.ToString();
    }

    public async Task<SessionResponse> PostRedeemTicketAsync(string ticket, CancellationToken cancellationToken = default)
    {
        string requestUri = $"authentication/redeem-ticket?ticket={Uri.EscapeDataString(ticket)}";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        SessionResponse sessionResponse = await GetContentAsync<SessionResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return sessionResponse;
    }

    public async Task ValidateSessionAsync(CancellationToken cancellationToken = default)
    {
        string requestUri = "authentication/validate-session";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task PostLogoutAsync(CancellationToken cancellationToken = default)
    {
        string requestUri = "authentication/logout";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task PostLogoutAllAsync(CancellationToken cancellationToken = default)
    {
        string requestUri = "authentication/logout-all";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<UserResponse> GetMeAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("users/me", cancellationToken).ConfigureAwait(false);
        UserResponse user = await GetContentAsync<UserResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return user;
    }

    public async Task PatchUserSettingsAsync(PatchUserSettingsRequest request, CancellationToken cancellationToken = default)
    {
        string requestUri = "users/me/settings";
        HttpResponseMessage responseMessage =
            await _HttpClient.PatchAsJsonAsync(requestUri, request, _JsonOptions, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<ContributionsPageResponse> GetContributionsAsync(
        GetContributionsRequest request, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(request);
        string requestUri = $"contributions?{query}";

        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        ContributionsPageResponse response = await GetContentAsync<ContributionsPageResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<HttpResponseMessage> PostUserBanAsync(
        CreateUserBanRequest request, CancellationToken cancellationToken = default)
    {
        return await _HttpClient.PostAsJsonAsync("userbans", request, _JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> RevertContributionsAsync(
        RevertContributionsRequest request, CancellationToken cancellationToken = default)
    {
        string requestUri = "contributions/revert";

        HttpResponseMessage responseMessage =
            await _HttpClient.PostAsJsonAsync(requestUri, request, _JsonOptions, cancellationToken).ConfigureAwait(false);

        return responseMessage;
    }

    public async Task<GuidelinesResponse> GetContributionGuidelinesAsync(CancellationToken cancellationToken = default)
    {
        string requestUri = "guidelines";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        GuidelinesResponse response = await GetContentAsync<GuidelinesResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<ConfigurationResponse> GetApiStateAsync(CancellationToken cancellationToken = default)
    {
        string requestUri = "configuration";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        ConfigurationResponse response = await GetContentAsync<ConfigurationResponse>(responseMessage, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task PatchConfigurationAsync(PatchConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        string requestUri = "configuration";
        HttpResponseMessage responseMessage =
            await _HttpClient.PatchAsJsonAsync(requestUri, request, _JsonOptions, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
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
