using Microsoft.AspNetCore.SignalR;

namespace LevelTagApi.Hubs;

public class LevelHub : Hub
{
    public async Task JoinLevelGroup(long levelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, levelId.ToString());
    }

    public async Task LeaveLevelGroup(long levelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, levelId.ToString());
    }
}