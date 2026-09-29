using Distags.Api.Contracts.Users;
using Distags.WebApp.DistagsApi;

namespace Distags.WebApp.Security;

public class CachedUserInfoProvider : IUserInfoProvider
{
    private readonly DistagsApiClient _DistagsApi;

    private bool _IsInitialized;
    private UserResponse? _CachedUser;

    public CachedUserInfoProvider(DistagsApiClient distagsApi)
    {
        _DistagsApi = distagsApi;
    }

    public async Task<UserResponse?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        if (!_IsInitialized)
            await InitializeAsync(cancellationToken);

        return _CachedUser;
    }

    private async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        ApiResult<UserResponse> apiResult = await _DistagsApi.GetMeAsync(cancellationToken);
        _CachedUser = apiResult.Value;
        _IsInitialized = true;
    }
}
