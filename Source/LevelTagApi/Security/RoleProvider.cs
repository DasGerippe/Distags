namespace LevelTagApi.Security;

public class RoleProvider
{
    public List<string> GetAvailableRoles()
    {
        return [Constants.Roles.Tagger, Constants.Roles.AutoTagger];
    }
}