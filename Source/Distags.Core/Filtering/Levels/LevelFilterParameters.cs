namespace Distags.Core.Filtering.Levels;

public class LevelFilterParameters
{
    public string? LevelFilter { get; set; }

    public string? CreatorFilter { get; set; }

    public List<string> IncludeTags { get; set; } = [];

    public List<string> ExcludeTags { get; set; } = [];
}