using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Management;

namespace Distags.Core.Logic;

public class LevelTagRemover
{
    private readonly TagChangeManager _TagChangeManager;

    public LevelTagRemover(TagChangeManager tagChangeManager)
    {
        _TagChangeManager = tagChangeManager;
    }

    public DomainResult<List<TagChange>> RemoveTagFromLevel(Level level, Tag tag)
    {
        if (!level.Tags.Contains(tag))
            return DomainResult<List<TagChange>>.Failure(DomainError.LevelTagDoesNotExist);

        List<TagChange> changes = [_TagChangeManager.CreateChange(TagChangeAction.Remove, tag)];

        Tag? fallbackTag = GetFallbackTagIfRequired(level, tag);
        if (fallbackTag != null)
        {
            if (fallbackTag == tag)
                return DomainResult<List<TagChange>>.Failure(DomainError.CannotRemoveFallbackTag);

            level.Tags.Add(fallbackTag);
            changes.Add(_TagChangeManager.CreateChange(TagChangeAction.Add, fallbackTag));
        }

        level.Tags.Remove(tag);

        return DomainResult<List<TagChange>>.Success(changes);
    }

    private Tag? GetFallbackTagIfRequired(Level level, Tag removedTag)
    {
        if (removedTag.LogicGroupId == null)
            return null;

        if (removedTag.LogicGroup?.Tags == null)
            throw new IncompleteEntityAggregateException("Incomplete tag aggregate.", nameof(removedTag));

        int levelLogicGroupTagCount = level.Tags.Count(tag => tag.LogicGroupId == removedTag.LogicGroupId);

        if (levelLogicGroupTagCount > 1)
            return null;

        Tag? fallbackTag = removedTag.LogicGroup.Tags.FirstOrDefault(tag => tag.IsFallback);
        return fallbackTag;
    }
}
