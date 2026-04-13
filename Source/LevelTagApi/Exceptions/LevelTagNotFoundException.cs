namespace LevelTagApi.Exceptions;

public class LevelTagNotFoundException : Exception
{
    public long LevelId { get; }

    public string TagName { get; }

    public LevelTagNotFoundException(long levelId, string tagName)
    {
        LevelId = levelId;
        TagName = tagName;
    }
}