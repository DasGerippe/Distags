using Distags.Contracts;

namespace Distags.WebApp.Models;

public class DisplayGroupWithTags
{
    public DisplayGroupDto DisplayGroup { get; }

    public IReadOnlyCollection<TagDto> Tags { get; }

    public bool AllTagsExclusive { get; }

    public DisplayGroupWithTags(DisplayGroupDto displayGroup, List<TagDto> tags)
    {
        DisplayGroup = displayGroup;
        Tags = tags.AsReadOnly();
        AllTagsExclusive = tags.All(tag => tag.IsExclusive);
    }
}
