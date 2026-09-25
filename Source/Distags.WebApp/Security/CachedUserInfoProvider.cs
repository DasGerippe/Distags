using Distags.Api.Contracts.Users;
using Distags.WebApp.Services;
using System.Net;

namespace Distags.WebApp.Security;

public class CachedUserInfoProvider : IUserInfoProvider
{
    private readonly DistagsApiClient _DistagsApi;
    private readonly ILogger<CachedUserInfoProvider> _Logger;

    private bool _IsInitialized;
    private UserResponse? _CachedUser;

    public CachedUserInfoProvider(DistagsApiClient distagsApi, ILogger<CachedUserInfoProvider> logger)
    {
        _DistagsApi = distagsApi;
        _Logger = logger;
    }

    public async Task<UserResponse?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        if (!_IsInitialized)
        {
            _CachedUser = await TryGetUserInfo(cancellationToken);
            _IsInitialized = true;
        }

        return _CachedUser;
    }

    private async Task<UserResponse?> TryGetUserInfo(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Trying to retrieve user info from API...");
        try
        {
            UserResponse user = await _DistagsApi.GetMeAsync(cancellationToken);
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
