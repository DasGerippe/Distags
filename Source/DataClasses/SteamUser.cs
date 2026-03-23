namespace DataClasses;

public class SteamUser
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Level> CreatedLevels { get; set; } = [];
}