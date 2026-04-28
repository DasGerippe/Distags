using DataClasses;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.DeleteApp;

public class DeleteAppCommandHandler : ICommandHandler<DeleteAppCommand>
{
    private readonly DbContext _DbContext;

    public DeleteAppCommandHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(DeleteAppCommand command)
    {
        App? app = await _DbContext.Set<App>().FirstOrDefaultAsync(app => app.Name == command.AppName).ConfigureAwait(false);
        if (app == null)
            throw new UnknownAppNameException(command.AppName);

        _DbContext.Remove(app);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}