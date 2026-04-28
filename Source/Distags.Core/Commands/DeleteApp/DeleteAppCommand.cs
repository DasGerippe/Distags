namespace Distags.Core.Commands.DeleteApp;

public class DeleteAppCommand : ICommand
{
    public string AppName { get; set; } = string.Empty;
}