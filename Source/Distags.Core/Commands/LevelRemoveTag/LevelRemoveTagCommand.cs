namespace Distags.Core.Commands.LevelRemoveTag;

public class LevelRemoveTagCommand : ICommand
{
    public long LevelId { get; set; }

    public string TagName { get; set; } = string.Empty;
}