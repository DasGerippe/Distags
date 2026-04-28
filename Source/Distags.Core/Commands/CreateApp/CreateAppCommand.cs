namespace Distags.Core.Commands.CreateApp;

public class CreateAppCommand : ICommand
{
    public string AppName { get; set; } = string.Empty;
}