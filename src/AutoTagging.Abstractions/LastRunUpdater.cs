using Distags.Core;
using Distags.Core.Commands.CreateOrUpdateAutoTagger;

namespace AutoTagging.Abstractions;

public class LastRunUpdater
{
    private readonly AutoTaggerInfo _AutoTaggerInfo;
    private readonly IRequestDispatcher _RequestDispatcher;

    public LastRunUpdater(AutoTaggerInfo autoTaggerInfo, IRequestDispatcher requestDispatcher)
    {
        _AutoTaggerInfo = autoTaggerInfo;
        _RequestDispatcher = requestDispatcher;
    }

    public Task UpdateLastRunAsync()
    {
        CreateOrUpdateAutoTaggerCommand command = new CreateOrUpdateAutoTaggerCommand
        {
            Guid = _AutoTaggerInfo.Guid,
            LastRunOn = DateTimeOffset.UtcNow,
        };
        return _RequestDispatcher.DispatchAsync(command);
    }
}
