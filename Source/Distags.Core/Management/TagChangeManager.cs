using Distags.Contracts;
using Distags.Core.Entities;

namespace Distags.Core.Management;

public class TagChangeManager
{
    public TagChange CreateChange(TagChangeAction action, Tag tag)
    {
        return new TagChange
        {
            Action = action,
            TagName = tag.Name,
        };
    }

    public List<TagChange> CreateChanges(Tag addedTag, IEnumerable<Tag> removedTags)
    {
        return
        [
            CreateChange(TagChangeAction.Add, addedTag),
            ..removedTags.Select(removedTag => CreateChange(TagChangeAction.Remove, removedTag)),
        ];
    }
}