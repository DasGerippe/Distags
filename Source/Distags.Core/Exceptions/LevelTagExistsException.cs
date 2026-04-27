namespace Distags.Core.Exceptions;

public class LevelTagExistsException : DistagsException
{
    public long LevelId { get; }

    public string TagName { get; }

    public LevelTagExistsException(long levelId, string tagName)
    {
        LevelId = levelId;
        TagName = tagName;
    }
}