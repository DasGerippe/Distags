namespace Distags.Core.Commands.SetLastCreatorRefreshDate;

public class SetLastCreatorRefreshDateCommand : ICommand
{
    public DateTimeOffset? LastCreatorRefreshDate { get; set; }
}
