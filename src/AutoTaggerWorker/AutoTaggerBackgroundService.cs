using AutoTagging;
using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoTaggerWorker;

public class AutoTaggerBackgroundService : BackgroundService
{
    private readonly AutoTaggerQueue _AutoTaggerQueue;
    private readonly IServiceScopeFactory _ScopeFactory;
    private readonly ILogger<AutoTaggerBackgroundService> _Logger;

    public AutoTaggerBackgroundService(
        AutoTaggerQueue autoTaggerQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<AutoTaggerBackgroundService> logger)
    {
        _AutoTaggerQueue = autoTaggerQueue;
        _ScopeFactory = scopeFactory;
        _Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Type? autoTaggerType = await GetNextAutoTaggerType(stoppingToken);
            if (autoTaggerType == null || stoppingToken.IsCancellationRequested)
                break;

            try
            {
                using IServiceScope scope = _ScopeFactory.CreateScope();

                AutoTaggerRunner autoTaggerRunner = scope.ServiceProvider.GetRequiredService<AutoTaggerRunner>();
                AutoTagger autoTagger = (AutoTagger)scope.ServiceProvider.GetRequiredService(autoTaggerType);

                await autoTaggerRunner.RunAsync(autoTagger, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "An error occurred while executing {autoTagger}.", autoTaggerType.Name);
            }
        }
    }

    private async Task<Type?> GetNextAutoTaggerType(CancellationToken stoppingToken)
    {
        try
        {
            return await _AutoTaggerQueue.DequeueAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}
