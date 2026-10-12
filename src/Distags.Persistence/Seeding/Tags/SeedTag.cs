namespace Distags.Persistence.Seeding.Tags;

public class SeedTag
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string DisplayGroupName { get; set; } = string.Empty;

    public string LogicGroupName { get; set; } = string.Empty;

    public bool IsExclusive { get; set; }

    public bool IsFallback { get; set; }

    public bool IsAutoTag { get; set; }

    public string Description { get; set; } = string.Empty;
}
