using Distags.Contracts;
using Distags.WebApp.Services;
using System.Net;

namespace Distags.WebApp.Security;

public class CachedUserInfoProvider : IUserInfoProvider
{
    private readonly DistagsApiClient _LevelTagApi;
    private readonly ILogger<CachedUserInfoProvider> _Logger;

    private bool _IsInitialized;
    private UserDto? _CachedUser;

    public CachedUserInfoProvider(DistagsApiClient levelTagApi, ILogger<CachedUserInfoProvider> logger)
    {
        _LevelTagApi = levelTagApi;
        _Logger = logger;
    }

    public async Task<UserDto?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        if (!_IsInitialized)
        {
            _CachedUser = await TryGetUserInfo(cancellationToken);
            _IsInitialized = true;
        }

        return _CachedUser;
    }

    private async Task<UserDto?> TryGetUserInfo(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Trying to retrieve user info from API...");
        try
        {
            UserDto user = await _LevelTagApi.GetMe(cancellationToken);
            _Logger.LogDebug("Retrieved user info.");
            return user;
        }
        catch (Exception ex)
        {
            if (ex is HttpRequestException { StatusCode: HttpStatusCode.Unauthorized })
                _Logger.LogDebug("User is not logged in.");
            else
                _Logger.LogDebug(ex, "An error occurred while trying to get user info.");

            return null;
        }
    }
}
