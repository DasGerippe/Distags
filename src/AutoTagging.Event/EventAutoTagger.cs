using AutoTagging.Abstractions;
using AutoTagging.Abstractions.Progress;
using AutoTagging.Event.Collection;
using Microsoft.Extensions.DependencyInjection;
using SteamWebApi;

namespace AutoTagging.Event;

public class EventAutoTagger : StrategyAutoTagger
{
    public override Guid Guid { get; } = new Guid("E8D84F78-9042-46E1-B9D8-F743C1B76269");

    public override TimeSpan? Interval => TimeSpan.FromMinutes(30);

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddSteamWebApi("SteamWebApi");

        services.AddTransient<ITaggingStrategy, CollectionTaggingStrategy>();
        services.AddTransient<EventCollectionScraper>();
        services.AddTransient<EventCollectionLevelsScraper>();

        services.AddProgressiveStrategy<UpdatedWorkshopLevelsStrategy, WorkshopTagsEventCalculator>("workshop-tags");
        services.AddProgressiveStrategy<UpdatedWorkshopLevelsStrategy, LevelNameEventCalculator>("name");
    }
}
