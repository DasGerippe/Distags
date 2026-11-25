using AutoTagging.Abstractions;
using Distags.Contracts;

namespace AutoTagging.MissingLevel;

public class MissingLevelCalculator : ITagDeltaCalculator
{
    private const string MissingLevelTag = "missing-level";

    public Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        TagDelta tagDelta = new TagDelta();
        if (level is not WorkshopLevelDto workshopLevel)
            return Task.FromResult(tagDelta);

        if (DistanceFileNameValidator.IsValidFileName(workshopLevel.FileName))
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
