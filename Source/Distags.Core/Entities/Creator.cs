namespace Distags.Core.Entities;

public class Creator
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string AvatarHash { get; set; } = string.Empty;

    public DateTimeOffset CachedOn { get; set; }

    public List<Level> CreatedLevels { get; set; } = [];
}
