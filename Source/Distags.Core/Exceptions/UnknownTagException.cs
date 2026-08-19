namespace Distags.Core.Exceptions;

public class UnknownTagException : DistagsException
{
    public string TagName { get; }

    public UnknownTagException(string tagName)
    {
        TagName = tagName;
    }
}
