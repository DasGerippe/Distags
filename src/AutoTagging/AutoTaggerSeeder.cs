using AutoTagging.Abstractions;
using Distags.Core;
using Distags.Core.Commands.CreateOrUpdateAutoTagger;

namespace AutoTagging;

public class AutoTaggerSeeder
{
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly IEnumerable<IAutoTagger> _AutoTaggers;

    public AutoTaggerSeeder(IRequestDispatcher requestDispatcher, IEnumerable<IAutoTagger> autoTaggers)
    {
        _RequestDispatcher = requestDispatcher;
        _AutoTaggers = autoTaggers;
    }

    public async Task CreateOrUpdateRegisteredAutoTaggers(CancellationToken cancellationToken = default)
    {
        foreach (IAutoTagger autoTagger in _AutoTaggers)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            CreateOrUpdateAutoTaggerCommand command = new CreateOrUpdateAutoTaggerCommand
            {
                Guid = autoTagger.Guid,
                Name = autoTagger.Name,
            };
            await _RequestDispatcher.DispatchAsync(command).ConfigureAwait(false);
        }
    }
}
