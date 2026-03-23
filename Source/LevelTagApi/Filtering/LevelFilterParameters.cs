namespace LevelTagApi.Filtering;

public class LevelFilterParameters
{
    public string? LevelFilter { get; set; }

    public string? CreatorFilter { get; set; }

    public string? IncludeTags { get; set; }

    public string? ExcludeTags { get; set; }
}