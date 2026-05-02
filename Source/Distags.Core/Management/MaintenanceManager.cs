namespace Distags.Core.Management;

public class MaintenanceManager : SettingManager<bool>
{
    protected override string SettingKey => "MaintenanceActive";

    protected override bool DefaultValue => false;

    public MaintenanceManager(DistagsDbContext dbContext) : base(dbContext)
    {
    }
}