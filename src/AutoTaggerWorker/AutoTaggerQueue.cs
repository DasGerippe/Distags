using System.Threading.Channels;

namespace AutoTaggerWorker;

public class AutoTaggerQueue
{
    private readonly Channel<Type> _Channel = Channel.CreateUnbounded<Type>(new UnboundedChannelOptions
    {
        SingleReader = true,
    });
    private readonly HashSet<Type> _Queued = [];
    private readonly Lock _Lock = new Lock();

    public bool TryEnqueue(Type autoTaggerType)
    {
        lock (_Lock)
        {
            if (!_Queued.Add(autoTaggerType))
                return false;
        }

        return _Channel.Writer.TryWrite(autoTaggerType);
    }

    public async Task<Type> DequeueAsync(CancellationToken cancellationToken)
    {
        Type autoTaggerType = await _Channel.Reader.ReadAsync(cancellationToken);
        lock (_Lock)
        {
            _Queued.Remove(autoTaggerType);
        }

        return autoTaggerType;
    }
}
