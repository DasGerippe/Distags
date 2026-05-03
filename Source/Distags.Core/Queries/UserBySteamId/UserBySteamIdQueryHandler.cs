using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBySteamId;

public class UserBySteamIdQueryHandler : IQueryHandler<UserBySteamIdQuery, UserDto?>
{
    private readonly DistagsDbContext _DbContext;

    public UserBySteamIdQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<UserDto?> HandleAsync(UserBySteamIdQuery query)
    {
        return await _DbContext.Set<User>()
            .Where(user => user.SteamId == query.SteamId)
            .Select(new UserMapper().Expression)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}