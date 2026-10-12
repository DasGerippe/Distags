using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Mapping;

namespace Distags.Core.Events;

public class LevelTagsChangedEvent : IAppEvent
{
    public LevelDto Level { get; }

    public IReadOnlyList<TagChangeDto> Changes { get; }

    public LevelTagsChangedEvent(Level level, IReadOnlyCollection<TagChange> changes)
        : this(LevelMapper.Map(level), TagChangeMapper.Map(changes))
    {
    }

    public LevelTagsChangedEvent(LevelDto level, List<TagChangeDto> changes)
    {
        Level = level;
        Changes = changes;
    }
}
