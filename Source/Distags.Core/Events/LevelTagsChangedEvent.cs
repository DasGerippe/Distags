using Distags.Contracts;
using System.Collections.ObjectModel;

namespace Distags.Core.Events;

public class LevelTagsChangedEvent : IAppEvent
{
    public long LevelId { get; }

    public IReadOnlyList<TagChangeDto> Changes { get; }

    public LevelTagsChangedEvent(long levelId, List<TagChangeDto> changes)
    {
        LevelId = levelId;
        Changes = new ReadOnlyCollection<TagChangeDto>(changes);
    }
}
