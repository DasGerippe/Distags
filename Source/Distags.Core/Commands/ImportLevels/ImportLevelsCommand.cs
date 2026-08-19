namespace Distags.Core.Commands.ImportLevels;

public class ImportLevelsCommand : ICommand
{
    public List<ScrapedLevel> ScrapedLevels { get; set; } = [];
}
