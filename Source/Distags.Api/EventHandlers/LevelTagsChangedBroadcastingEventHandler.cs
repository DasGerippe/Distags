using Distags.Api.Hubs;
using Distags.Core.Events;
using Microsoft.AspNetCore.SignalR;

namespace Distags.Api.EventHandlers;

public class LevelTagsChangedBroadcastingEventHandler : IAppEventHandler<LevelTagsChangedEvent>
{
    private readonly IHubContext<LevelHub, ILevelHubClient> _LevelHub;

    public LevelTagsChangedBroadcastingEventHandler(IHubContext<LevelHub, ILevelHubClient> levelHub)
    {
        _LevelHub = levelHub;
    }

    public async Task HandleAsync(LevelTagsChangedEvent appEvent)
    {
        await _LevelHub.Clients.Group(appEvent.LevelId.ToString()).TagsChanged(appEvent.Changes);
    }
}
