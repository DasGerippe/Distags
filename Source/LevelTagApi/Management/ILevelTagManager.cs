using DataClasses;

namespace LevelTagApi.Management;

public interface ILevelTagManager
{
    Task AddTagToLevelAsync(Level level, Tag tag);

    Task RemoveTagFromLevelAsync(LevelTag levelTag);
}