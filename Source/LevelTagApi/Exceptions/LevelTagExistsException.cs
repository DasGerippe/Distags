namespace LevelTagApi.Exceptions;

public class LevelTagExistsException : Exception
{
    public long LevelId { get; }

    public string TagName { get; }

    public LevelTagExistsException(long levelId, string tagName)
    {
        LevelId = levelId;
        TagName = tagName;
    }
}