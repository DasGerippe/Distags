using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.ApiState;

public class ApiStateService : IApiStateService
{
    private const string MaintenanceSettingKey = "MaintenanceActive";

    private readonly IServiceScopeFactory _ScopeFactory;

    private bool? _MaintenanceActive;

    public ApiStateService(IServiceScopeFactory scopeFactory)
    {
        _ScopeFactory = scopeFactory;
    }

    public async Task<bool> GetMaintenanceStateAsync()
    {
        if (_MaintenanceActive == null)
        {
            using IServiceScope scope = _ScopeFactory.CreateScope();
            LevelTagDbContext dbContext = scope.ServiceProvider.GetRequiredService<LevelTagDbContext>();

            Setting? maintenanceSetting = await LoadMaintenanceSetting(dbContext);
            _MaintenanceActive = maintenanceSetting != null && bool.Parse(maintenanceSetting.Value!);
        }

        return _MaintenanceActive.Value;
    }

    public async Task SetMaintenanceStateAsync(bool maintenanceActive)
    {
        if (_MaintenanceActive == maintenanceActive)
            return;

        _MaintenanceActive = maintenanceActive;

        using IServiceScope scope = _ScopeFactory.CreateScope();
        LevelTagDbContext dbContext = scope.ServiceProvider.GetRequiredService<LevelTagDbContext>();

        Setting? maintenanceSetting = await LoadMaintenanceSetting(dbContext);

        if (maintenanceSetting == null)
        {
            maintenanceSetting = new Setting
            {
                Key = MaintenanceSettingKey,
            };
            dbContext.Add(maintenanceSetting);
        }

        maintenanceSetting.Value = maintenanceActive.ToString();

        await dbContext.SaveChangesAsync();
    }

    private async Task<Setting?> LoadMaintenanceSetting(LevelTagDbContext dbContext)
    {
        return await dbContext.Set<Setting>().FirstOrDefaultAsync(setting => setting.Key == MaintenanceSettingKey);
    }
}