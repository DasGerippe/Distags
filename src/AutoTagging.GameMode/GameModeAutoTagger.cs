using AutoTagging.Abstractions;
using AutoTagging.Abstractions.Progress;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.GameMode;

public class GameModeAutoTagger : StrategyAutoTagger
{
    public override Guid Guid { get; } = new Guid("AD96F858-91F3-46E7-B2C2-19DAFBA0DFDF");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(10);

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddProgressiveStrategy<UpdatedLevelsStrategy, GameModeCalculator>(string.Empty);
    }
}
