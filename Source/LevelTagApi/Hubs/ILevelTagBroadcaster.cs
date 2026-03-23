namespace LevelTagApi.Hubs;

public interface ILevelTagBroadcaster
{
    Task BroadcastLevelTagAddedAsync(long levelId, string tagName);

    Task BroadcastLevelTagRemovedAsync(long levelId, string tagName);
}