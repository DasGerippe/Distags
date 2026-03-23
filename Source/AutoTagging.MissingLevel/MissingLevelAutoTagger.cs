using AutoTagging.Abstractions;
using DataClasses;

namespace AutoTagging.MissingLevel;

public class MissingLevelAutoTagger : AutoTagger
{
    private const string MissingLevelTag = "missing-level";

    public override TaggingStrategy TaggingStrategy => TaggingStrategy.NewLevels;

    public override Task<TagDelta> CalculateTagDeltaAsync(Level level, CancellationToken cancellationToken = default)
    {
        TagDelta tagDelta = new TagDelta();

        if (DistanceFileNameValidator.IsValidFileName(level.FileName))
        {
            if (level.Tags.Any(tag => tag.Name == MissingLevelTag))
                tagDelta.RemoveTags.Add(MissingLevelTag);
        }
        else
        {
            if (!level.Tags.Any(tag => tag.Name == MissingLevelTag))
                tagDelta.AddTags.Add(MissingLevelTag);
        }

        return Task.FromResult(tagDelta);
    }
}