namespace Distags.Core.Exceptions;

public class UnknownAppNameException : DistagsException
{
    public string AppName { get; }

    public UnknownAppNameException(string appName)
    {
        AppName = appName;
    }
}
