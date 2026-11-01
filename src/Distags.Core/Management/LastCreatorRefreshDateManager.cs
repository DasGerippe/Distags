namespace Distags.Core.Management;

public class LastCreatorRefreshDateManager : SettingManager<DateTimeOffset?>
{
    protected override string SettingKey => "LastCreatorRefreshDate";

    protected override DateTimeOffset? DefaultValue => null;

    public LastCreatorRefreshDateManager(DistagsDbContext dbContext) : base(dbContext)
    {
    }
}
