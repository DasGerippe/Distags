namespace Distags.Core.Commands.ImportOfficialLevels;

public class ImportOfficialLevelsCommand : ICommand
{
    public List<ScrapedOfficialLevel> ScrapedOfficialLevels { get; set; } = [];
}
