namespace Distags.Core.Commands.ImportWorkshopLevels;

public class ImportWorkshopLevelsCommand : ICommand
{
    public List<ScrapedWorkshopLevel> ScrapedWorkshopLevels { get; set; } = [];
}
