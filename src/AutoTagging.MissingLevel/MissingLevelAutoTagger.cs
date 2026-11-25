using AutoTagging.Abstractions;
using AutoTagging.Abstractions.Progress;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.MissingLevel;

public class MissingLevelAutoTagger : StrategyAutoTagger
{
    public override Guid Guid { get; } = new Guid("074598C5-1C0A-4302-95C4-F93B5572E399");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(5);

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddProgressiveStrategy<NewWorkshopLevelsStrategy, MissingLevelCalculator>(string.Empty);
    }
}
