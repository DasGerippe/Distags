namespace Distags.Core.Commands.ImportCreators;

public class ImportCreatorsCommand : ICommand
{
    public List<ScrapedCreator> ScrapedCreators { get; set; } = [];
}