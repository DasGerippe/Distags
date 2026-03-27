using DataClasses;

namespace LevelTagApi.Management;

public class UserBanPermissionChecker : IUserBanPermissionChecker
{
    public bool CanBan(User banningUser, User bannedUser)
    {
        if (bannedUser.Roles.Contains("Administrator"))
            return false;

        if (bannedUser.Roles.Contains("Moderator"))
            return banningUser.Roles.Contains("Administrator");

        return true;
    }
}