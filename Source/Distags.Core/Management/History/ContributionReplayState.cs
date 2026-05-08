using Distags.Contracts;
using Distags.Core.Entities;

namespace Distags.Core.Management.History;

public class ContributionReplayState
{
    private readonly TagChangeManager _TagChangeManager;
    private readonly HashSet<Tag> _CombinableTags = [];

    private readonly Dictionary<(int Exclusivity, long? GroupId), Tag> _ExclusiveTags = [];

    public ContributionReplayState(TagChangeManager tagChangeManager)
    {
        _TagChangeManager = tagChangeManager;
    }

    public List<TagChange> ApplyContribution(Contribution contribution)
    {
        return contribution.Tag.Exclusivity == null
            ? ApplyCombinableTagContribution(contribution.Action, contribution.Tag)
            : ApplyExclusiveTagContribution(contribution.Action, contribution.Tag);
    }

    public IEnumerable<Tag> GetTagsForLevel()
    {
        return _CombinableTags.Concat(_ExclusiveTags.Values);
    }

    private List<TagChange> ApplyCombinableTagContribution(TagChangeAction action, Tag tag)
    {
        bool tagStateChanged =
            action == TagChangeAction.Add && _CombinableTags.Add(tag) ||
            action == TagChangeAction.Remove && _CombinableTags.Remove(tag);

        List<TagChange> changes = tagStateChanged
            ? [_TagChangeManager.CreateChange(action, tag)]
            : [];

        return changes;
    }

    private List<TagChange> ApplyExclusiveTagContribution(TagChangeAction action, Tag tag)
    {
        List<TagChange> changes = new List<TagChange>(2);

        if (action == TagChangeAction.Add)
        {
            if (_ExclusiveTags.Remove((tag.Exclusivity!.Value, tag.GroupId), out Tag? existingTag))
                changes.Add(_TagChangeManager.CreateChange(TagChangeAction.Remove, existingTag));

            if (_ExclusiveTags.TryAdd((tag.Exclusivity!.Value, tag.GroupId), tag))
                changes.Add(_TagChangeManager.CreateChange(TagChangeAction.Add, tag));
        }
        else if (action == TagChangeAction.Remove)
        {
            if (_ExclusiveTags.TryGetValue((tag.Exclusivity!.Value, tag.GroupId), out Tag? existingTag) && existingTag == tag)
            {
                _ExclusiveTags.Remove((tag.Exclusivity!.Value, tag.GroupId));
                changes.Add(_TagChangeManager.CreateChange(TagChangeAction.Remove, tag));
            }
        }

        return changes;
    }
}