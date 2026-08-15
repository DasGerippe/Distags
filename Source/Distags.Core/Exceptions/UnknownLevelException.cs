namespace Distags.Core.Exceptions;

public class UnknownLevelException : DistagsException
{
    public long LevelId { get; }

    public UnknownLevelException(long levelId)
    {
        LevelId = levelId;
    }
}
