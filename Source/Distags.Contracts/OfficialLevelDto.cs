namespace Distags.Contracts;

public class OfficialLevelDto : LevelDto
{
    public override LevelType Type => LevelType.Official;

    public string Slug { get; set; } = string.Empty;

    public string CreatorName { get; set; } = string.Empty;

    public List<string> GameModes { get; set; } = [];
}
