using AutoTagging.Abstractions;
using Distags.Contracts;

namespace AutoTagging.Event;

public class WorkshopTagsEventCalculator : ITagDeltaCalculator
{
    private static readonly Dictionary<string, string> EventMappings = new Dictionary<string, string>
    {
        { "Survive the Editor", "ste-1" },
        { "Survive the Editor 2", "ste-2" },
        { "Distance Advent Calendar 2014", "dac-2014" },
        { "Distance Advent Calendar 2015", "dac-2015" },
        { "Distance Advent Calendar 2016", "dac-2016" },
        { "Distance Advent Calendar 2017", "dac-2017" },
        { "Distance Advent Calendar 2018", "dac-2018" },
        { "Distance Advent Calendar 2019", "dac-2019" },
    };

    public Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        if (level is not WorkshopLevelDto workshopLevel)
            return Task.FromResult(new TagDelta());

        string? desiredEventTagName = workshopLevel.WorkshopTags
            .Join(
                EventMappings,
                workshopTag => workshopTag,
                eventMapping => eventMapping.Key,
                (_, eventMapping) => eventMapping.Value)
            .FirstOrDefault();

        TagDelta tagDelta = new TagDelta();
        if (desiredEventTagName != null && !level.Tags.Contains(desiredEventTagName))
            tagDelta.AddTags.Add(desiredEventTagName);

        return Task.FromResult(tagDelta);
    }
}
