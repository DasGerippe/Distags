using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBanBySteamId;

public class UserBanBySteamIdQueryHandler : IQueryHandler<UserBanBySteamIdQuery, UserBanDto?>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IEntityProjector<UserBan, UserBanDto> _UserBanProjector;

    public UserBanBySteamIdQueryHandler(DistagsDbContext dbContext, IEntityProjector<UserBan, UserBanDto> userBanProjector)
    {
        _DbContext = dbContext;
        _UserBanProjector = userBanProjector;
    }

    public async Task<UserBanDto?> HandleAsync(UserBanBySteamIdQuery query)
    {
        return await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == query.SteamId)
            .Select(_UserBanProjector.Projection)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}