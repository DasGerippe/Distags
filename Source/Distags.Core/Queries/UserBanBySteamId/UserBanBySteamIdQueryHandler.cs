using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBanBySteamId;

public class UserBanBySteamIdQueryHandler : IQueryHandler<UserBanBySteamIdQuery, UserBanDto?>
{
    private readonly DistagsDbContext _DbContext;

    public UserBanBySteamIdQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<UserBanDto?> HandleAsync(UserBanBySteamIdQuery query)
    {
        return await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == query.SteamId)
            .Select(new UserBanMapper().Expression)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}