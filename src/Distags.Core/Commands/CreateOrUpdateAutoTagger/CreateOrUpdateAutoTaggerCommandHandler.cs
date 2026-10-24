using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.CreateOrUpdateAutoTagger;

public class CreateOrUpdateAutoTaggerCommandHandler : ICommandHandler<CreateOrUpdateAutoTaggerCommand>
{
    private readonly DistagsDbContext _DbContext;

    public CreateOrUpdateAutoTaggerCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(CreateOrUpdateAutoTaggerCommand command)
    {
        AutoTagger? autoTagger = await _DbContext.Set<AutoTagger>()
            .Where(autoTagger => autoTagger.Guid == command.Guid)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (autoTagger == null)
        {
            autoTagger = new AutoTagger
            {
                Guid = command.Guid,
            };
            _DbContext.Add(autoTagger);
        }

        if (command.Name.HasValue)
            autoTagger.Name = command.Name.Value;

        if (command.LastRunOn.HasValue)
            autoTagger.LastRunOn = command.LastRunOn.Value;

        if (command.State.HasValue)
            autoTagger.State = command.State.Value;

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
