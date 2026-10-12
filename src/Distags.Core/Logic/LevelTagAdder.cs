using Distags.Core.Entities;
using Distags.Core.Management;

namespace Distags.Core.Logic;

public class LevelTagAdder
{
    private readonly TagChangeManager _TagChangeManager;

    public LevelTagAdder(TagChangeManager tagChangeManager)
    {
        _TagChangeManager = tagChangeManager;
    }

    public DomainResult<List<TagChange>> AddTagToLevel(Level level, Tag tag)
    {
        if (level.Tags.Contains(tag))
            return DomainResult<List<TagChange>>.Failure(DomainError.LevelTagAlreadyExists);

        List<Tag> conflictingTags = GetConflictingTags(level, tag);
        foreach (Tag conflictingTag in conflictingTags)
        {
            level.Tags.Remove(conflictingTag);
        }

        level.Tags.Add(tag);

        return DomainResult<List<TagChange>>.Success(_TagChangeManager.CreateChanges(tag, conflictingTags));
    }

    private List<Tag> GetConflictingTags(Level level, Tag addedTag)
    {
        if (addedTag.LogicGroupId == null)
            return [];

        List<Tag> conflictingTags = level.Tags
            .Where(tag =>
                tag.LogicGroupId == addedTag.LogicGroupId &&
                (tag.IsExclusive || addedTag.IsExclusive))
            .ToList();

        return conflictingTags;
    }
}
