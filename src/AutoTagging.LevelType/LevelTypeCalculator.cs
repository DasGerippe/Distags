using AutoTagging.Abstractions;
using Distags.Contracts;

namespace AutoTagging.LevelType;

public class LevelTypeCalculator : ITagDeltaCalculator
{
    private const string OfficialTagName = "official";
    private const string WorkshopTagName = "workshop";

    public Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        TagDelta tagDelta = new TagDelta();

        string desiredLevelTypeTagName = level.Type switch
        {
            Distags.Contracts.LevelType.Official => OfficialTagName,
            Distags.Contracts.LevelType.Workshop => WorkshopTagName,
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };

        if (!level.Tags.Contains(desiredLevelTypeTagName))
            tagDelta.AddTags.Add(desiredLevelTypeTagName);

        return Task.FromResult(tagDelta);
    }
}
