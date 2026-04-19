using LevelTagApi.Dtos.Users;

namespace LevelTagWebApp.Security;

public interface IUserInfoProvider
{
    Task<UserDto?> GetUserInfoAsync(CancellationToken cancellationToken = default);
}