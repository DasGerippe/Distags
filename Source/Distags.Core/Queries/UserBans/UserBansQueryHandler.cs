using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UserBans;

public class UserBansQueryHandler : IQueryHandler<UserBansQuery, List<UserBanDto>>
{
    private readonly DistagsDbContext _DbContext;

    public UserBansQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<UserBanDto>> HandleAsync(UserBansQuery query)
    {
        return await _DbContext.Set<UserBan>()
            .OrderByDescending(userBan => userBan.BannedOn)
            .Select(new UserBanMapper().Expression)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}