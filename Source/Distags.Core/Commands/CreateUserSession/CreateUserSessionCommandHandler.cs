using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.CreateUserSession;

public class CreateUserSessionCommandHandler : ICommandHandler<CreateUserSessionCommand>
{
    private readonly DistagsDbContext _DbContext;

    public CreateUserSessionCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(CreateUserSessionCommand command)
    {
        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.UserSteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (user == null)
            throw new UnknownUserException(command.UserSteamId);

        UserSession userSession = new UserSession
        {
            User = user,
            Token = command.SessionToken,
            LastSeenOn = DateTimeOffset.UtcNow,
        };

        _DbContext.Add(userSession);
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
