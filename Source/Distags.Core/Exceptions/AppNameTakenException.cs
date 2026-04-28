namespace Distags.Core.Exceptions;

public class AppNameTakenException : DistagsException
{
    public string AppName { get; }

    public AppNameTakenException(string appName)
    {
        AppName = appName;
    }
}