namespace LevelTagApi.Exceptions;

public class UnknownLevelException : Exception
{
    public long LevelId { get; }

    public UnknownLevelException(long levelId)
    {
        LevelId = levelId;
    }
}