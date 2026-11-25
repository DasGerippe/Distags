using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.Abstractions;

public abstract class BaseAutoTagger : IAutoTagger
{
    public abstract Guid Guid { get; }

    public virtual string Name => field ??= GetType().Name;

    public abstract TimeSpan? Interval { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(new AutoTaggerInfo
        {
            Guid = Guid,
            Name = Name,
        });
        services.AddTransient<StateStore>();
        services.AddTransient<TagChanger>();
        services.AddTransient<LastRunUpdater>();
    }

    public abstract Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
