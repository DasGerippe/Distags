namespace LevelTagApi.Management;

public interface ILevelTagManager
{
    Task AddTagToLevelAsync(long levelId, string tagName);

    Task RemoveTagFromLevelAsync(long levelId, string tagName);
}