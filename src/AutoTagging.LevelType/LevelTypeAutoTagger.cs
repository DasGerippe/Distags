using AutoTagging.Abstractions;
using AutoTagging.Abstractions.Progress;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.LevelType;

public class LevelTypeAutoTagger : StrategyAutoTagger
{
    public override Guid Guid { get; } = new Guid("2E6677C5-DF38-498A-843E-0468B21CFF94");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(10);

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddProgressiveStrategy<NewLevelsStrategy, LevelTypeCalculator>(string.Empty);
    }
}
