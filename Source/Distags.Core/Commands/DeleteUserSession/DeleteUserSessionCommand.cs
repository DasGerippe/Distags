namespace Distags.Core.Commands.DeleteUserSession;

public class DeleteUserSessionCommand : ICommand
{
    public string SessionToken { get; set; } = string.Empty;
}