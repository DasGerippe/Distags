using DataClasses;

namespace LevelTagApi.Management;

public interface IUserManager
{
    Task<User?> GetCurrentUserAsync(params string[] includes);

    Task<UserSession?> GetCurrentUserSessionAsync(params string[] includes);
}