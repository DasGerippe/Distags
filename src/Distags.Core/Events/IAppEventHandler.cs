namespace Distags.Core.Events;

public interface IAppEventHandler<TApplicationEvent>
    where TApplicationEvent : IAppEvent
{
    Task HandleAsync(TApplicationEvent appEvent);
}
