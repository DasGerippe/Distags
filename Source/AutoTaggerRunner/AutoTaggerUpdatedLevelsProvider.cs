using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace AutoTaggerRunner;

public class AutoTaggerUpdatedLevelsProvider : IAutoTaggerLevelProvider
{
    private readonly IDbContextFactory<LevelTagDbContext> _DbContextFactory;

    public AutoTaggerUpdatedLevelsProvider(IDbContextFactory<LevelTagDbContext> dbContextFactory)
    {
        _DbContextFactory = dbContextFactory;
    }

    public async Task<List<Level>> GetLevelsToTagAsync(AutoTaggerState state)
    {
        LevelTagDbContext dbContext = await _DbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        await using ConfiguredAsyncDisposable awaitedDbContext = dbContext.ConfigureAwait(false);

        return await dbContext.Set<Level>()
            .Include(level => level.Tags)
            .Where(level => level.UpdatedOn > state.LastAutoTagging)
            .OrderBy(level => level.UpdatedOn)
            .ToListAsync().ConfigureAwait(false);
    }
}