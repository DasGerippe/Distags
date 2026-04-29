using DataClasses;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.UpdateUserSettings;

public class UpdateUserSettingsCommandHandler : ICommandHandler<UpdateUserSettingsCommand>
{
    private readonly DbContext _DbContext;

    public UpdateUserSettingsCommandHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(UpdateUserSettingsCommand command)
    {
        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.SteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (user == null)
            throw new UnknownUserException(command.SteamId);

        if (command.AcceptsGuidelines.HasValue)
            user.AcceptsGuidelines = command.AcceptsGuidelines;

        if (command.IsAnonymous.HasValue)
            user.IsAnonymous = command.IsAnonymous;

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}