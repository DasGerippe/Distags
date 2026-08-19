using Distags.Core.Management;

namespace Distags.Core.Queries.Maintenance;

public class MaintenanceQueryHandler : IQueryHandler<MaintenanceQuery, bool>
{
    private readonly MaintenanceManager _MaintenanceManager;

    public MaintenanceQueryHandler(MaintenanceManager maintenanceManager)
    {
        _MaintenanceManager = maintenanceManager;
    }

    public Task<bool> HandleAsync(MaintenanceQuery query)
    {
        return _MaintenanceManager.GetSettingValueAsync();
    }
}
