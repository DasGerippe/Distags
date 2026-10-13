using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBySessionToken;

public class UserBySessionTokenQueryHandler : IQueryHandler<UserBySessionTokenQuery, UserDto?>
{
    private readonly DistagsDbContext _DbContext;

    public UserBySessionTokenQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<UserDto?> HandleAsync(UserBySessionTokenQuery query)
    {
        UserDto? userDto = await _DbContext.Set<UserSession>()
            .Where(userSession =>
                userSession.Token == query.SessionToken &&
                userSession.User.SteamId != null)
            .Select(userSession => userSession.User)
            .SelectDto()
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return userDto;
    }
}
