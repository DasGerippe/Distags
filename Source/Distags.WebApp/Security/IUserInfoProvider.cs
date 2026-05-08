using Distags.Contracts;

namespace Distags.WebApp.Security;

public interface IUserInfoProvider
{
    Task<UserDto?> GetUserInfoAsync(CancellationToken cancellationToken = default);
}