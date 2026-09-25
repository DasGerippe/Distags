using Distags.Api.Contracts.Tags;
using Distags.Api.Hubs;
using Distags.Api.Mapping;
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
        string levelGroupName = LevelKeyGenerator.GetKey(appEvent.Level);
        List<TagChangeResponse> changes = TagChangeMapper.Map(appEvent.Changes);
        await _LevelHub.Clients.Group(levelGroupName).TagsChanged(changes);
    }
}
