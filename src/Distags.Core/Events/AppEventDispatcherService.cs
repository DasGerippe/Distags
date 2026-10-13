using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Distags.Core.Events;

public class AppEventDispatcherService : BackgroundService
{
    private readonly AppEventQueue _AppEventQueue;
    private readonly IServiceProvider _ServiceProvider;

    public AppEventDispatcherService(AppEventQueue appEventQueue, IServiceProvider serviceProvider)
    {
        _AppEventQueue = appEventQueue;
        _ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (IAppEvent appEvent in _AppEventQueue.DequeueAllAsync(stoppingToken).ConfigureAwait(false))
            {
                using IServiceScope scope = _ServiceProvider.CreateScope();

                Type handlerType = typeof(IAppEventHandler<>).MakeGenericType(appEvent.GetType());

                IEnumerable<object?> appEventHandlers = scope.ServiceProvider.GetServices(handlerType);

                foreach (dynamic? appEventHandler in appEventHandlers)
                {
                    if (appEventHandler != null)
                        await appEventHandler.HandleAsync((dynamic)appEvent).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
