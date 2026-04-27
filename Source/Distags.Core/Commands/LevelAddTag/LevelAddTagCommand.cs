namespace Distags.Core.Commands.LevelAddTag;

public class LevelAddTagCommand : ICommand
{
    public long LevelId { get; set; }

    public string TagName { get; set; } = string.Empty;
}