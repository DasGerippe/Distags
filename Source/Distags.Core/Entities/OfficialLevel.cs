using Distags.Contracts;

namespace Distags.Core.Entities;

public class OfficialLevel : Level
{
    public string Slug { get; set; } = string.Empty;

    public string CreatorName { get; set; } = string.Empty;

    public List<string> GameModes { get; set; } = [];

    public OfficialLevel() : base(LevelType.Official)
    {
    }
}
