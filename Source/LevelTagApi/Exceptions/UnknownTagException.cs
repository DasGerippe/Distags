namespace LevelTagApi.Exceptions;

public class UnknownTagException : Exception
{
    public string TagName { get; }

    public UnknownTagException(string tagName)
    {
        TagName = tagName;
    }
}