using System.Threading.Channels;

namespace Distags.Core.Events;

public class AppEventQueue
{
    private readonly Channel<IAppEvent> _Channel = Channel.CreateUnbounded<IAppEvent>();

    public void Enqueue(IAppEvent appEvent)
    {
        _Channel.Writer.TryWrite(appEvent);
    }

    public IAsyncEnumerable<IAppEvent> DequeueAllAsync(CancellationToken cancellationToken = default)
    {
        return _Channel.Reader.ReadAllAsync(cancellationToken);
    }
}
