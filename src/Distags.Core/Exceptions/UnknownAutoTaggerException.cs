namespace Distags.Core.Exceptions;

public class UnknownAutoTaggerException : DistagsException
{
    public Guid AutoTaggerGuid { get; }

    public UnknownAutoTaggerException(Guid autoTaggerGuid)
    {
        AutoTaggerGuid = autoTaggerGuid;
    }
}
