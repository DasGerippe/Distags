using Distags.Contracts;

namespace Distags.Core.Filtering.Levels;

public class LevelFilterParameters
{
    public LevelType? Type { get; set; }

    public string? LevelName { get; set; }

    public string? CreatorName { get; set; }

    public List<string> IncludeTags { get; set; } = [];

    public List<string> ExcludeTags { get; set; } = [];
}
