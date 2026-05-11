using Distags.Contracts;
using Distags.Core.Entities;

namespace Distags.Core.Services;

public class UserBanPermissionChecker
{
    public bool CanBan(User banningUser, User bannedUser)
    {
        if (bannedUser.Roles.Contains(Constants.UserRoles.Administrator))
            return false;

        if (bannedUser.Roles.Contains(Constants.UserRoles.Moderator))
            return banningUser.Roles.Contains(Constants.UserRoles.Administrator);

        return true;
    }
}