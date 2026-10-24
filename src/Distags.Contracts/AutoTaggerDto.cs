namespace Distags.Contracts;

public class AutoTaggerDto
{
    public Guid Guid { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTimeOffset? LastRunOn { get; set; }

    public string? State { get; set; }
}
