using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBySessionToken;

public class UserBySessionTokenQueryHandler : IQueryHandler<UserBySessionTokenQuery, UserDto?>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IEntityProjector<User, UserDto> _UserProjector;

    public UserBySessionTokenQueryHandler(DistagsDbContext dbContext, IEntityProjector<User, UserDto> userProjector)
    {
        _DbContext = dbContext;
        _UserProjector = userProjector;
    }

    public async Task<UserDto?> HandleAsync(UserBySessionTokenQuery query)
    {
        UserDto? userDto = await _DbContext.Set<UserSession>()
            .Where(userSession =>
                userSession.Token == query.SessionToken &&
                userSession.User.SteamId != null)
            .Select(userSession => userSession.User)
            .Select(_UserProjector.Projection)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return userDto;
    }
}