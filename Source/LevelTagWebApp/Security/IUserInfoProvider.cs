using LevelTagApi.Dtos;

namespace LevelTagWebApp.Security;

public interface IUserInfoProvider
{
    Task<UserDto?> GetUserInfoAsync(CancellationToken cancellationToken = default);
}