namespace Distags.Core.Entities;

public class AutoTagger
{
    public long Id { get; set; }

    public Guid Guid { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? State { get; set; }
}
