using Distags.Api.Contracts.Users;

namespace Distags.WebApp.Security;

public interface IUserInfoProvider
{
    Task<UserResponse?> GetUserInfoAsync(CancellationToken cancellationToken = default);
}
