namespace Distags.Core.Exceptions;

public class LevelTagNotFoundException : DistagsException
{
    public long LevelId { get; }

    public string TagName { get; }

    public LevelTagNotFoundException(long levelId, string tagName)
    {
        LevelId = levelId;
        TagName = tagName;
    }
}