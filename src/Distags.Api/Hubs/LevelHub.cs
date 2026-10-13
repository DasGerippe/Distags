using Microsoft.AspNetCore.SignalR;

namespace Distags.Api.Hubs;

public class LevelHub : Hub<ILevelHubClient>
{
    public async Task JoinLevelGroup(string levelGroupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, levelGroupName);
    }

    public async Task LeaveLevelGroup(string levelGroupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, levelGroupName);
    }
}
