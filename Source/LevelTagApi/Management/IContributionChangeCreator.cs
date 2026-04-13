using DataClasses;

namespace LevelTagApi.Management;

public interface IContributionChangeCreator
{
    ContributionChange CreateChange(ChangeAction action, Tag removedTag);

    List<ContributionChange> CreateChanges(Tag addedTag, IEnumerable<Tag> removedTags);
}