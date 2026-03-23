using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace AutoTaggerRunner;

public class AutoTaggerNewLevelsProvider : IAutoTaggerLevelProvider
{
    private readonly IDbContextFactory<LevelTagDbContext> _DbContextFactory;

    public AutoTaggerNewLevelsProvider(IDbContextFactory<LevelTagDbContext> dbContextFactory)
    {
        _DbContextFactory = dbContextFactory;
    }

    public async Task<List<Level>> GetLevelsToTagAsync(AutoTaggerState state)
    {
        LevelTagDbContext dbContext = await _DbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = dbContext.ConfigureAwait(false);

        return await dbContext.Set<Level>()
            .Where(level => level.CreatedOn > state.LastAutoTagging)
            .OrderBy(level => level.CreatedOn)
            .ToListAsync().ConfigureAwait(false);
    }
}