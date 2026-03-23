using Microsoft.AspNetCore.SignalR;

namespace LevelTagApi.Hubs;

public class LevelTagBroadcaster : ILevelTagBroadcaster
{
    private readonly IHubContext<LevelHub, ILevelHubClient> _LevelHub;

    public LevelTagBroadcaster(IHubContext<LevelHub, ILevelHubClient> levelHub)
    {
        _LevelHub = levelHub;
    }

    public Task BroadcastLevelTagAddedAsync(long levelId, string tagName)
    {
        return GetLevelGroup(levelId).TagAdded(tagName);
    }

    public Task BroadcastLevelTagRemovedAsync(long levelId, string tagName)
    {
        return GetLevelGroup(levelId).TagRemoved(tagName);
    }

    private ILevelHubClient GetLevelGroup(long levelId)
    {
        return _LevelHub.Clients.Group(levelId.ToString());
    }
}