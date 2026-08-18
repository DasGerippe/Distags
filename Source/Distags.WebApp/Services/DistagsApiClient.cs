using Distags.Api.Contracts;
using Distags.Api.Contracts.Authentication;
using Distags.Api.Contracts.Configuration;
using Distags.Api.Contracts.Contributions;
using Distags.Api.Contracts.Guidelines;
using Distags.Api.Contracts.Levels;
using Distags.Api.Contracts.UserBans;
using Distags.Api.Contracts.Users;
using Distags.Contracts;
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

    public async Task<List<DisplayGroupDto>> GetDisplayGroupsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("DisplayGroups", cancellationToken).ConfigureAwait(false);
        List<DisplayGroupDto> displayGroups = await GetContentAsync<List<DisplayGroupDto>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return displayGroups;
    }

    public async Task<List<LogicGroupDto>> GetLogicGroupsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("LogicGroups", cancellationToken).ConfigureAwait(false);
        List<LogicGroupDto> logicGroups = await GetContentAsync<List<LogicGroupDto>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return logicGroups;
    }

    public async Task<List<TagDto>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("Tags", cancellationToken).ConfigureAwait(false);
        List<TagDto> logicGroups = await GetContentAsync<List<TagDto>>(responseMessage, cancellationToken).ConfigureAwait(false);
        return logicGroups;
    }

    public async Task<LevelsPage> BrowseLevelsAsync(
        BrowseLevelsParameters parameters, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(parameters);
        string requestUri = $"levels?{query}";

        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        LevelsPage levelsResponse = await GetContentAsync<LevelsPage>(responseMessage, cancellationToken).ConfigureAwait(false);
        return levelsResponse;
    }

    public async Task<LevelDto?> GetLevelAsync(long levelId, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        LevelDto level = await GetContentAsync<LevelDto>(responseMessage, cancellationToken).ConfigureAwait(false);
        return level;
    }

    public Task PostLevelTag(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        return _HttpClient.PutAsync(requestUri, null, cancellationToken);
    }

    public Task DeleteLevelTag(long levelId, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = $"levels/{levelId}/tags/{tagName}";
        return _HttpClient.DeleteAsync(requestUri, cancellationToken);
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

    public async Task ValidateSession(CancellationToken cancellationToken = default)
    {
        string requestUri = "authentication/validate-session";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task PostLogout(CancellationToken cancellationToken = default)
    {
        string requestUri = "authentication/logout";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task PostLogoutAll(CancellationToken cancellationToken = default)
    {
        string requestUri = "authentication/logout-all";
        HttpResponseMessage responseMessage = await _HttpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<UserDto> GetMe(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync("users/me", cancellationToken).ConfigureAwait(false);
        UserDto user = await GetContentAsync<UserDto>(responseMessage, cancellationToken).ConfigureAwait(false);
        return user;
    }

    public async Task PatchUserSettingsAsync(PatchUserSettingsParameters parameters, CancellationToken cancellationToken = default)
    {
        string requestUri = "users/me/settings";
        HttpResponseMessage responseMessage =
            await _HttpClient.PatchAsJsonAsync(requestUri, parameters, _JsonOptions, cancellationToken).ConfigureAwait(false);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<ContributionsPage> GetContributionsAsync(
        GetContributionsParameters parameters, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(parameters);
        string requestUri = $"contributions?{query}";

        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        ContributionsPage response = await GetContentAsync<ContributionsPage>(responseMessage, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<HttpResponseMessage> PostUserBan(
        CreateUserBanDto createUserBanDto, CancellationToken cancellationToken = default)
    {
        return await _HttpClient.PostAsJsonAsync("userbans", createUserBanDto, _JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> RevertContributionsAsync(
        RevertContributionsParameters parameters, CancellationToken cancellationToken = default)
    {
        string requestUri = "contributions/revert";

        HttpResponseMessage responseMessage =
            await _HttpClient.PostAsJsonAsync(requestUri, parameters, _JsonOptions, cancellationToken).ConfigureAwait(false);

        return responseMessage;
    }

    public async Task<GuidelinesDto> GetContributionGuidelinesAsync(CancellationToken cancellationToken = default)
    {
        string requestUri = "guidelines";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        GuidelinesDto response = await GetContentAsync<GuidelinesDto>(responseMessage, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<ConfigurationDto> GetApiStateAsync(CancellationToken cancellationToken = default)
    {
        string requestUri = "configuration";
        HttpResponseMessage responseMessage = await _HttpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        ConfigurationDto response = await GetContentAsync<ConfigurationDto>(responseMessage, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task PatchConfigurationAsync(PatchConfigurationParameters parameters, CancellationToken cancellationToken = default)
    {
        string requestUri = "configuration";
        HttpResponseMessage responseMessage =
            await _HttpClient.PatchAsJsonAsync(requestUri, parameters, _JsonOptions, cancellationToken).ConfigureAwait(false);
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
