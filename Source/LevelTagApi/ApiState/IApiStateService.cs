namespace LevelTagApi.ApiState;

public interface IApiStateService
{
    Task<bool> GetMaintenanceStateAsync();

    Task SetMaintenanceStateAsync(bool maintenanceActive);
}