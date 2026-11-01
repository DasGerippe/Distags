using Distags.Core.Management;

namespace Distags.Core.Commands.SetLastCreatorRefreshDate;

public class SetLastCreatorRefreshDateCommandHandler : ICommandHandler<SetLastCreatorRefreshDateCommand>
{
    private readonly LastCreatorRefreshDateManager _LastCreatorRefreshDateManager;

    public SetLastCreatorRefreshDateCommandHandler(LastCreatorRefreshDateManager lastCreatorRefreshDateManager)
    {
        _LastCreatorRefreshDateManager = lastCreatorRefreshDateManager;
    }

    public async Task HandleAsync(SetLastCreatorRefreshDateCommand command)
    {
        await _LastCreatorRefreshDateManager.SetSettingValueAsync(command.LastCreatorRefreshDate).ConfigureAwait(false);
    }
}
