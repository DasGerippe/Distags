using Distags.Api.Contracts;
using Distags.Api.Contracts.Authentication;
using Distags.Api.Contracts.Configuration;
using Distags.Api.Contracts.Contributions;
using Distags.Api.Contracts.DisplayGroups;
using Distags.Api.Contracts.Guidelines;
using Distags.Api.Contracts.Levels;
using Distags.Api.Contracts.LogicGroups;
using Distags.Api.Contracts.Stats;
using Distags.Api.Contracts.Tags;
using Distags.Api.Contracts.UserBans;
using Distags.Api.Contracts.Users;
using Distags.WebApp.Security;
using HttpUtils;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Distags.WebApp.DistagsApi;

public class DistagsApiClient : IDisposable
{
    private readonly HttpClient _HttpClient;
    private readonly DistagsWebAppSettings _WebAppSettings;
    private readonly ISessionTokenProvider _SessionTokenProvider;
    private readonly JsonSerializerOptions _JsonOptions;

    public Uri LevelHubEndpoint { get; }

    public Uri ContributionHubEndpoint { get; }

    public DistagsApiClient(HttpClient httpClient, DistagsWebAppSettings webAppSettings, ISessionTokenProvider sessionTokenProvider)
    {
        _HttpClient = httpClient;
        _WebAppSettings = webAppSettings;
        _SessionTokenProvider = sessionTokenProvider;

        _JsonOptions = new JsonSerializerOptions(JsonSerializerOptions.Web);
        _JsonOptions.Converters.Add(new PatchValueConverterFactory());
        _JsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        _HttpClient.BaseAddress = new Uri(webAppSettings.DistagsApiUrlInternal);
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

    public Task<ApiResult<List<DisplayGroupResponse>>> GetDisplayGroupsAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync("displaygroups", cancellationToken)
            .ToApiResultAsync<List<DisplayGroupResponse>>(cancellationToken);
    }

    public Task<ApiResult<List<LogicGroupResponse>>> GetLogicGroupsAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync("logicgroups", cancellationToken)
            .ToApiResultAsync<List<LogicGroupResponse>>(cancellationToken);
    }

    public Task<ApiResult<List<TagResponse>>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync("tags", cancellationToken)
            .ToApiResultAsync<List<TagResponse>>(cancellationToken);
    }

    public Task<ApiResult<LevelsPageResponse>> BrowseLevelsAsync(
        BrowseLevelsRequest request, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(request);
        string requestUri = $"levels?{query}";
        return _HttpClient.GetAsync(requestUri, cancellationToken)
            .ToApiResultAsync<LevelsPageResponse>(cancellationToken);
    }

    public Task<ApiResult<OfficialLevelResponse>> GetOfficialLevelAsync(string levelSlug, CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync($"levels/official/{levelSlug}", cancellationToken)
            .ToApiResultAsync<OfficialLevelResponse>(cancellationToken);
    }

    public Task<ApiResult<WorkshopLevelResponse>> GetWorkshopLevelAsync(long levelWorkshopId, CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync($"levels/workshop/{levelWorkshopId}", cancellationToken)
            .ToApiResultAsync<WorkshopLevelResponse>(cancellationToken);
    }

    public Task<ApiResult> PutLevelTagAsync(LevelResponse level, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = GetLevelTagUri(level, tagName);
        return _HttpClient.PutAsync(requestUri, null, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    public Task<ApiResult> DeleteLevelTagAsync(LevelResponse level, string tagName, CancellationToken cancellationToken = default)
    {
        string requestUri = GetLevelTagUri(level, tagName);
        return _HttpClient.DeleteAsync(requestUri, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    private string GetLevelTagUri(LevelResponse level, string tagName)
    {
        return $"levels/{level.Key}/tags/{tagName}";
    }

    public string GetSteamLoginEndpoint(string? returnUrl = null)
    {
        UriBuilder uriBuilder = new UriBuilder(_WebAppSettings.DistagsApiUrlExternal);
        uriBuilder.Path = "authentication/steam-login";
        if (!string.IsNullOrEmpty(returnUrl))
            uriBuilder.Query = $"returnUrl={returnUrl}";

        return uriBuilder.ToString();
    }

    public Task<ApiResult<SessionResponse>> PostRedeemTicketAsync(string ticket, CancellationToken cancellationToken = default)
    {
        string requestUri = $"authentication/redeem-ticket?ticket={Uri.EscapeDataString(ticket)}";
        return _HttpClient.PostAsync(requestUri, null, cancellationToken)
            .ToApiResultAsync<SessionResponse>(cancellationToken);
    }

    public Task<ApiResult> PostLogoutAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.PostAsync("authentication/logout", null, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    public Task<ApiResult> PostLogoutAllAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.PostAsync("authentication/logout-all", null, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    public Task<ApiResult<UserResponse>> GetMeAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync("users/me", cancellationToken)
            .ToApiResultAsync<UserResponse>(cancellationToken);
    }

    public Task<ApiResult> PatchUserSettingsAsync(PatchUserSettingsRequest request, CancellationToken cancellationToken = default)
    {
        return _HttpClient.PatchAsJsonAsync("users/me/settings", request, _JsonOptions, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    public Task<ApiResult<ContributionsPageResponse>> GetContributionsAsync(
        GetContributionsRequest request, CancellationToken cancellationToken = default)
    {
        string query = QueryBuilder.BuildQueryString(request);
        string requestUri = $"contributions?{query}";
        return _HttpClient.GetAsync(requestUri, cancellationToken)
            .ToApiResultAsync<ContributionsPageResponse>(cancellationToken);
    }

    public Task<ApiResult> PostUserBanAsync(
        CreateUserBanRequest request, CancellationToken cancellationToken = default)
    {
        return _HttpClient.PostAsJsonAsync("userbans", request, _JsonOptions, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    public Task<ApiResult> RevertContributionsAsync(
        RevertContributionsRequest request, CancellationToken cancellationToken = default)
    {
        return _HttpClient.PostAsJsonAsync("contributions/revert", request, _JsonOptions, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    public Task<ApiResult<GuidelinesResponse>> GetContributionGuidelinesAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync("guidelines", cancellationToken)
            .ToApiResultAsync<GuidelinesResponse>(cancellationToken);
    }

    public Task<ApiResult<ConfigurationResponse>> GetApiStateAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync("configuration", cancellationToken)
            .ToApiResultAsync<ConfigurationResponse>(cancellationToken);
    }

    public Task<ApiResult> PatchConfigurationAsync(PatchConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        return _HttpClient.PatchAsJsonAsync("configuration", request, _JsonOptions, cancellationToken)
            .ToApiResultAsync(cancellationToken);
    }

    public Task<ApiResult<StatsSummaryResponse>> GetStatsSummaryAsync(CancellationToken cancellationToken = default)
    {
        return _HttpClient.GetAsync("stats/summary", cancellationToken)
            .ToApiResultAsync<StatsSummaryResponse>(cancellationToken);
    }
}
