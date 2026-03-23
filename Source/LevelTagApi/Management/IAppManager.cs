using DataClasses;

namespace LevelTagApi.Management;

public interface IAppManager
{
    Task<App?> GetCurrentAppAsync(params string[] includes);
}