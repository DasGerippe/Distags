using System.Net;
using LevelTagApi.Dtos;
using LevelTagWebApp.Services;

namespace LevelTagWebApp.Security;

public sealed class UserInfoProvider : IUserInfoProvider, IDisposable
{
    private readonly ISessionTokenProvider _SessionTokenProvider;
    private readonly LevelTagApiClient _LevelTagApi;
    private readonly ILogger<UserInfoProvider> _Logger;

    public long? SteamId { get; private set; }

    public string? SteamName { get; private set; }

    public string? SteamAvatarHash { get; private set; }

    public IEnumerable<string> Roles { get; private set; } = [];

    public event EventHandler? UserInfoChanged;

    public UserInfoProvider(
        ISessionTokenProvider sessionTokenProvider,
        LevelTagApiClient levelTagApi,
        ILogger<UserInfoProvider> logger)
    {
        _SessionTokenProvider = sessionTokenProvider;
        _LevelTagApi = levelTagApi;
        _Logger = logger;

        _SessionTokenProvider.SessionTokenChanged += OnSessionTokenChanged;
    }

    public void Dispose()
    {
        _SessionTokenProvider.SessionTokenChanged -= OnSessionTokenChanged;
    }

    private async void OnSessionTokenChanged(object? sender, EventArgs e)
    {
        try
        {
            if (_SessionTokenProvider.SessionToken == null)
                ClearUserInfo();
            else
                await TryPopulateUserInfoFromLevelTagApi();
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "An error occurred while trying to handle changed SessionToken.");
        }
    }

    private async Task TryPopulateUserInfoFromLevelTagApi()
    {
        try
        {
            UserDto user = await _LevelTagApi.GetUser();


            RaiseUserInfoChanged();
        }
        catch (HttpRequestException httpRequestEx) when (httpRequestEx.StatusCode == HttpStatusCode.Unauthorized)
        {
            _Logger.LogInformation("Unauthorized to get user with current SessionToken.");
            _SessionTokenProvider.SessionToken = null;
        }
        catch (Exception ex)
        {
            
        }
    }

    private void ClearUserInfo()
    {
        SteamId = null;
        SteamName = null;
        SteamAvatarHash = null;
        Roles = [];

        RaiseUserInfoChanged();
    }

    private void RaiseUserInfoChanged()
    {
        UserInfoChanged?.Invoke(this, EventArgs.Empty);
    }
}