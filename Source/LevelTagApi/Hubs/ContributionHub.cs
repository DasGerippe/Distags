using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LevelTagApi.Hubs;

public class ContributionHub : Hub<IContributionHubClient>
{
    internal const string UsersGroupName = "Users";
    internal const string ModeratorsGroupName = "Moderators";

    public override async Task OnConnectedAsync()
    {
        if (Context.User != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupForUser(Context.User));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User != null)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupForUser(Context.User));
    }

    private static string GetGroupForUser(ClaimsPrincipal user)
    {
        return user.IsInRole(Constants.Roles.Moderator)
            ? ModeratorsGroupName
            : UsersGroupName;
    }
}