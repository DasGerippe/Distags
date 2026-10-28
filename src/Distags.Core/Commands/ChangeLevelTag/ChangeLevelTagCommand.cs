using Distags.Contracts;

namespace Distags.Core.Commands.ChangeLevelTag;

public abstract class ChangeLevelTagCommand : ICommand
{
    public string TagName { get; set; } = string.Empty;

    public TagChangeAction Action { get; set; }

    public long? UserSteamId { get; set; }

    public Guid? AutoTaggerGuid { get; set; }
}
