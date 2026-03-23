using LevelTagApi.Dtos;
using LevelTagWebApp.Services;

namespace LevelTagWebApp.Security;

public class CachedUserInfoProvider : IUserInfoProvider
{
    private readonly LevelTagApiClient _LevelTagApi;
    private readonly ILogger<CachedUserInfoProvider> _Logger;

    private bool _IsInitialized;
    private UserDto? _CachedUser;

    public CachedUserInfoProvider(LevelTagApiClient levelTagApi, ILogger<CachedUserInfoProvider> logger)
    {
        _LevelTagApi = levelTagApi;
        _Logger = logger;
    }

    public async Task<UserDto?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        if (!_IsInitialized)
        {
            await TryCacheUserInfo(cancellationToken);
            _IsInitialized = true;
        }

        return _CachedUser;
    }

    private async Task TryCacheUserInfo(CancellationToken cancellationToken = default)
    {
        try
        {
            _CachedUser = await _LevelTagApi.GetMe(cancellationToken);
        }
        catch (Exception ex)
        {
            _Logger.LogDebug(ex, "An error occurred while trying to get user info.");
        }
    }
}