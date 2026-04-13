using DataClasses;

namespace LevelTagApi.Management;

public class ContributionReplayState
{
    private readonly IContributionChangeCreator _ContributionChangeCreator;
    private readonly HashSet<Tag> _CombinableTags = [];

    private readonly Dictionary<(int Exclusivity, long? GroupId), Tag> _ExclusiveTags = [];

    public ContributionReplayState(IContributionChangeCreator contributionChangeCreator)
    {
        _ContributionChangeCreator = contributionChangeCreator;
    }

    public List<ContributionChange> ApplyContribution(Contribution contribution)
    {
        return contribution.Tag.Exclusivity == null
            ? ApplyCombinableTagContribution(contribution.Action, contribution.Tag)
            : ApplyExclusiveTagContribution(contribution.Action, contribution.Tag);
    }

    public IEnumerable<Tag> GetTagsForLevel()
    {
        return _CombinableTags.Concat(_ExclusiveTags.Values);
    }

    private List<ContributionChange> ApplyCombinableTagContribution(ChangeAction action, Tag tag)
    {
        bool tagStateChanged =
            action == ChangeAction.AddTag && _CombinableTags.Add(tag) ||
            action == ChangeAction.RemoveTag && _CombinableTags.Remove(tag);

        List<ContributionChange> changes = tagStateChanged
            ? [_ContributionChangeCreator.CreateChange(action, tag)]
            : [];

        return changes;
    }

    private List<ContributionChange> ApplyExclusiveTagContribution(ChangeAction action, Tag tag)
    {
        List<ContributionChange> changes = new List<ContributionChange>(2);

        if (action == ChangeAction.AddTag)
        {
            if (_ExclusiveTags.Remove((tag.Exclusivity!.Value, tag.GroupId), out Tag? existingTag))
                changes.Add(_ContributionChangeCreator.CreateChange(ChangeAction.RemoveTag, existingTag));

            if (_ExclusiveTags.TryAdd((tag.Exclusivity!.Value, tag.GroupId), tag))
                changes.Add(_ContributionChangeCreator.CreateChange(ChangeAction.AddTag, tag));
        }
        else if (action == ChangeAction.RemoveTag)
        {
            if (_ExclusiveTags.TryGetValue((tag.Exclusivity!.Value, tag.GroupId), out Tag? existingTag) && existingTag == tag)
            {
                _ExclusiveTags.Remove((tag.Exclusivity!.Value, tag.GroupId));
                changes.Add(_ContributionChangeCreator.CreateChange(ChangeAction.RemoveTag, tag));
            }
        }

        return changes;
    }
}