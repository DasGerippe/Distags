namespace Distags.Core.Commands.UpdateUserSessionLastSeen;

public class UpdateUserSessionLastSeenCommand : ICommand
{
    public string SessionToken { get; set; } = string.Empty;
}
