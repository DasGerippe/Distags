using AutoTagging.Abstractions;
using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;

namespace AutoTagging.LevelType;

public class LevelTypeAutoTagger : AutoTagger
{
    private const string OfficialTagName = "official";
    private const string WorkshopTagName = "workshop";

    public override ITaggingStrategy TaggingStrategy { get; } = new NewLevelsTaggingStrategy();

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        TagDelta tagDelta = new TagDelta();

        string? desiredLevelTypeTagName = level.Type switch
        {
            Distags.Contracts.LevelType.Official => OfficialTagName,
            Distags.Contracts.LevelType.Workshop => WorkshopTagName,
            _ => null,
        };

        if (desiredLevelTypeTagName != null && !level.Tags.Contains(desiredLevelTypeTagName))
            tagDelta.AddTags.Add(desiredLevelTypeTagName);

        return Task.FromResult(tagDelta);
    }
}
