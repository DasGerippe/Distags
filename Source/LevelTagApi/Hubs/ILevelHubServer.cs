namespace LevelTagApi.Hubs;

public interface ILevelHubServer
{
    Task SendTagAddedMessageAsync(long levelId, string tagName);

    Task SendTagRemovedMessageAsync(long levelId, string tagName);
}