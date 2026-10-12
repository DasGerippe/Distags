using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Projection;
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
            .SelectDto()
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}
