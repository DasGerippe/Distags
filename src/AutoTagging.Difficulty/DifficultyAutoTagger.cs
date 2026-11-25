using AutoTagging.Abstractions;
using AutoTagging.Abstractions.Progress;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.Difficulty;

public class DifficultyAutoTagger : StrategyAutoTagger
{
    public override Guid Guid { get; } = new Guid("2F5E210F-22C8-4BD3-B2C3-35655E3EC8F0");

    public override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(10);

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddProgressiveStrategy<UpdatedWorkshopLevelsStrategy, DifficultyCalculator>(string.Empty);
    }
}
