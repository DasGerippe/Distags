namespace Distags.Contracts;

public class TagDto
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }

    public int? Exclusivity { get; set; }

    public bool IsAutoTag { get; set; }
}