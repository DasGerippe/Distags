using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Distags.Core.Commands.SetAutoTaggerStateObject;

public class SetAutoTaggerStateObjectCommandHandler : ICommandHandler<SetAutoTaggerStateObjectCommand>
{
    private readonly DistagsDbContext _DbContext;

    public SetAutoTaggerStateObjectCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(SetAutoTaggerStateObjectCommand command)
    {
        AutoTaggerState? state = await _DbContext.Set<AutoTaggerState>()
            .Where(state => state.AutoTagger.Guid == command.AutoTaggerGuid && state.Key == command.StateKey)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (state == null)
        {
            AutoTagger? autoTagger = await _DbContext.Set<AutoTagger>()
                .Where(autoTagger => autoTagger.Guid == command.AutoTaggerGuid)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            autoTagger ??= new AutoTagger { Guid = command.AutoTaggerGuid };

            state = new AutoTaggerState
            {
                AutoTagger = autoTagger,
                Key = command.StateKey,
            };

            _DbContext.Add(state);
        }

        state.Value = command.StateValue == null
            ? null
            : JsonSerializer.Serialize(command.StateValue, command.StateType);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
