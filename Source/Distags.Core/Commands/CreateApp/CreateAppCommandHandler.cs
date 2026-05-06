using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.CreateApp;

public class CreateAppCommandHandler : ICommandHandler<CreateAppCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly TokenGenerator _TokenGenerator;

    public CreateAppCommandHandler(DistagsDbContext dbContext, TokenGenerator tokenGenerator)
    {
        _DbContext = dbContext;
        _TokenGenerator = tokenGenerator;
    }

    public async Task HandleAsync(CreateAppCommand command)
    {
        App? app = await _DbContext.Set<App>().FirstOrDefaultAsync(app => app.Name == command.AppName).ConfigureAwait(false);
        if (app != null)
            throw new AppNameTakenException(command.AppName);

        app = new App
        {
            Name = command.AppName,
            ApiKey = _TokenGenerator.GenerateToken(32),
        };

        _DbContext.Add(app);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}