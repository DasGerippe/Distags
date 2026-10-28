using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.AutoTaggerByGuid;

public class AutoTaggerByGuidQueryHandler : IQueryHandler<AutoTaggerByGuidQuery, AutoTaggerDto?>
{
    private readonly DistagsDbContext _DbContext;

    public AutoTaggerByGuidQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<AutoTaggerDto?> HandleAsync(AutoTaggerByGuidQuery query)
    {
        return await _DbContext.Set<AutoTagger>()
            .Where(autoTagger => autoTagger.Guid == query.AutoTaggerGuid)
            .SelectDto()
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}
