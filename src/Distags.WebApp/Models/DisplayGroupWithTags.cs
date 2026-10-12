using Distags.Api.Contracts.DisplayGroups;
using Distags.Api.Contracts.Tags;

namespace Distags.WebApp.Models;

public class DisplayGroupWithTags
{
    public DisplayGroupResponse DisplayGroup { get; }

    public IReadOnlyCollection<TagResponse> Tags { get; }

    public bool AllTagsExclusive { get; }

    public DisplayGroupWithTags(DisplayGroupResponse displayGroup, List<TagResponse> tags)
    {
        DisplayGroup = displayGroup;
        Tags = tags.AsReadOnly();
        AllTagsExclusive = tags.All(tag => tag.IsExclusive);
    }
}
