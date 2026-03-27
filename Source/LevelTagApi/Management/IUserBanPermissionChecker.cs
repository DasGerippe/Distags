using DataClasses;

namespace LevelTagApi.Management;

public interface IUserBanPermissionChecker
{
    bool CanBan(User banningUser, User bannedUser);
}