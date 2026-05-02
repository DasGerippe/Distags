using Distags.Core.Dtos;

namespace Distags.Core.Commands.ChangeLevelTag;

public class ChangeLevelTagCommand : ICommand
{
    public long LevelId { get; set; }

    public string TagName { get; set; } = string.Empty;

    public long? UserSteamId { get; set; }

    public string? AppName { get; set; }

    public ChangeActionDto ChangeAction { get; set; }
}