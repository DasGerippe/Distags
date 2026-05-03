using Distags.Core.Management;

namespace Distags.Core.Commands.SetMaintenance;

public class SetMaintenanceCommandHandler : ICommandHandler<SetMaintenanceCommand>
{
    private readonly MaintenanceManager _MaintenanceManager;

    public SetMaintenanceCommandHandler(MaintenanceManager maintenanceManager)
    {
        _MaintenanceManager = maintenanceManager;
    }

    public Task HandleAsync(SetMaintenanceCommand command)
    {
        return _MaintenanceManager.SetSettingValueAsync(command.MaintenanceActive);
    }
}