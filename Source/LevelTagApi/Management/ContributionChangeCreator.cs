using DataClasses;

namespace LevelTagApi.Management;

public class ContributionChangeCreator : IContributionChangeCreator
{
    public ContributionChange CreateChange(ChangeAction action, Tag tag)
    {
        return new ContributionChange
        {
            Action = action,
            TagName = tag.Name,
        };
    }

    public List<ContributionChange> CreateChanges(Tag addedTag, IEnumerable<Tag> removedTags)
    {
        return
        [
            CreateChange(ChangeAction.AddTag, addedTag),
            ..removedTags.Select(removedTag => CreateChange(ChangeAction.RemoveTag, removedTag)),
        ];
    }
}