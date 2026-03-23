using Microsoft.AspNetCore.SignalR;

namespace LevelTagApi.Hubs;

public class LevelHubServer : ILevelHubServer
{
    private readonly IHubContext<LevelHub> _LevelHub;

    public LevelHubServer(IHubContext<LevelHub> levelHub)
    {
        _LevelHub = levelHub;
    }

    public Task SendTagAddedMessageAsync(long levelId, string tagName)
    {
        return GetLevelGroup(levelId).SendAsync("TagAdded", tagName);
    }

    public Task SendTagRemovedMessageAsync(long levelId, string tagName)
    {
        return GetLevelGroup(levelId).SendAsync("TagRemoved", tagName);
    }

    private IClientProxy GetLevelGroup(long levelId)
    {
        return _LevelHub.Clients.Group(levelId.ToString());
    }
}