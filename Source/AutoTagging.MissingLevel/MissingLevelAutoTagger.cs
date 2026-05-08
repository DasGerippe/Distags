using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core.Mapping;

namespace AutoTagging.MissingLevel;

public class MissingLevelAutoTagger : AutoTagger
{
    private const string MissingLevelTag = "missing-level";

    public override TaggingStrategy TaggingStrategy => TaggingStrategy.NewLevels;

    public override LevelProjectionIncludes RequiredIncludes =>
        LevelProjectionIncludes.FileName |
        LevelProjectionIncludes.Tags;

    public override Task<TagDelta> CalculateTagDeltaAsync(LevelProjectionDto level, CancellationToken cancellationToken = default)
    {
        if (level.FileName == null || level.Tags == null)
            throw new ArgumentException("Required properties of level not initialized.", nameof(level));

        TagDelta tagDelta = new TagDelta();

        if (DistanceFileNameValidator.IsValidFileName(level.FileName))
        {
            if (level.Tags.Contains(MissingLevelTag))
                tagDelta.RemoveTags.Add(MissingLevelTag);
        }
        else
        {
            if (!level.Tags.Contains(MissingLevelTag))
                tagDelta.AddTags.Add(MissingLevelTag);
        }

        return Task.FromResult(tagDelta);
    }
}