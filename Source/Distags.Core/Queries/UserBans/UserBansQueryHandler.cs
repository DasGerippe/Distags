using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBans;

public class UserBansQueryHandler : IQueryHandler<UserBansQuery, List<UserBanDto>>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IEntityProjector<UserBan, UserBanDto> _UserBanProjector;

    public UserBansQueryHandler(DistagsDbContext dbContext, IEntityProjector<UserBan, UserBanDto> userBanProjector)
    {
        _DbContext = dbContext;
        _UserBanProjector = userBanProjector;
    }

    public async Task<List<UserBanDto>> HandleAsync(UserBansQuery query)
    {
        return await _DbContext.Set<UserBan>()
            .OrderByDescending(userBan => userBan.BannedOn)
            .Select(_UserBanProjector.Projection)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}