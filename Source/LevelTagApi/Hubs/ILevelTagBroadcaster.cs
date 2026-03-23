using DataClasses;

namespace LevelTagApi.Hubs;

public interface ILevelTagBroadcaster
{
    Task BroadcastLevelTagChangesAsync(IEnumerable<LevelTag> addedLevelTags, IEnumerable<LevelTag> removedLevelTags);
}