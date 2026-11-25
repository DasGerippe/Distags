namespace Distags.Core.Entities;

public class AutoTaggerState
{
    public long AutoTaggerId { get; set; }

    public required AutoTagger AutoTagger { get; set; }

    public required string Key { get; set; }

    public string? Value { get; set; }
}
