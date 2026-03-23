namespace LevelTagApi.Hubs;

public interface ILevelHubClient
{
    Task TagAdded(string tagName);

    Task TagRemoved(string tagName);
}