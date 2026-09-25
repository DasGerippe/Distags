namespace Distags.Core.Commands.ImportOfficialLevels;

public class ScrapedOfficialLevel
{
    public string Name { get; set; } = string.Empty;

    public string CreatorName { get; set; } = string.Empty;

    public List<string> GameModes { get; set; } = [];
}
