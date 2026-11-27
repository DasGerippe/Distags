using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.Length;

public class LengthAutoTagger : StrategyAutoTagger
{
    public override Guid Guid { get; } = new Guid("4764EBF2-C170-4A10-A9B9-EAFB164AB053");

    public override TimeSpan? Interval { get; } = TimeSpan.FromHours(6);

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddTransient<ITaggingStrategy, LengthTaggingStrategy>();
        services.AddTransient<SeekrDbReader>();
    }
}
