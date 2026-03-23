namespace LevelTagApi.Security;

public class RoleProvider
{
    public List<string> GetAvailableRoles()
    {
        return [Constants.Auth.TaggerRole, Constants.Auth.AutoTaggerRole];
    }
}