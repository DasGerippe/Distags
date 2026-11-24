using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutoTagging.Abstractions;

public abstract class StrategyAutoTagger : BaseAutoTagger
{
    public override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        LastRunUpdater lastRunUpdater = serviceProvider.GetRequiredService<LastRunUpdater>();
        IEnumerable<ITaggingStrategy> taggingStrategies = serviceProvider.GetServices<ITaggingStrategy>();
        ILogger<StrategyAutoTagger> logger = serviceProvider.GetRequiredService<ILogger<StrategyAutoTagger>>();

        foreach (ITaggingStrategy taggingStrategy in taggingStrategies)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await taggingStrategy.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while executing '{taggingStrategy}'.", taggingStrategy.GetType().Name);
            }
        }

        await lastRunUpdater.UpdateLastRunAsync().ConfigureAwait(false);
    }
}
