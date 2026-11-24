using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Distags.Core.Queries.AutoTaggerStateObject;

public class AutoTaggerStateObjectQueryHandler : IQueryHandler<AutoTaggerStateObjectQuery, object?>
{
    private readonly DistagsDbContext _DbContext;

    public AutoTaggerStateObjectQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<object?> HandleAsync(AutoTaggerStateObjectQuery query)
    {
        AutoTaggerState? state = await _DbContext.Set<AutoTaggerState>()
            .Where(state => state.AutoTagger.Guid == query.AutoTaggerGuid && state.Key == query.StateKey)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return state?.Value == null
            ? null
            : JsonSerializer.Deserialize(state.Value, query.StateType);
    }
}
