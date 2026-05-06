using DataClasses;
using Distags.Contracts;
using Distags.Core.Mapping;
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
            .Select(new UserMapper().Expression)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return userDto;
    }
}