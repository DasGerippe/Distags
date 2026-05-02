using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBySteamId;

public class UserBySteamIdQueryHandler : IQueryHandler<UserBySteamIdQuery, UserDto?>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IEntityProjector<User, UserDto> _UserProjector;

    public UserBySteamIdQueryHandler(DistagsDbContext dbContext, IEntityProjector<User, UserDto> userProjector)
    {
        _DbContext = dbContext;
        _UserProjector = userProjector;
    }

    public async Task<UserDto?> HandleAsync(UserBySteamIdQuery query)
    {
        return await _DbContext.Set<User>()
            .Where(user => user.SteamId == query.SteamId)
            .Select(_UserProjector.Projection)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}