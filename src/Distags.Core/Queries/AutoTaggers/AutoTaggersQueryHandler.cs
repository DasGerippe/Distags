using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.AutoTaggers;

public class AutoTaggersQueryHandler : IQueryHandler<AutoTaggersQuery, List<AutoTaggerDto>>
{
    private readonly DistagsDbContext _DbContext;

    public AutoTaggersQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<AutoTaggerDto>> HandleAsync(AutoTaggersQuery query)
    {
        return await _DbContext.Set<AutoTagger>()
            .SelectDto()
            .ToListAsync()
            .ConfigureAwait(false);
    }
}
