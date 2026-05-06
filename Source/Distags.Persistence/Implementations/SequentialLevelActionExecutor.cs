using Distags.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Distags.Persistence.Implementations;

public class SequentialLevelActionExecutor : ISequentialLevelActionExecutor
{
    private readonly DistagsDbContext _DbContext;

    public SequentialLevelActionExecutor(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task ExecuteSequentialForLevelAsync(long levelId, Func<Task> action)
    {
        await using IDbContextTransaction transaction = await _DbContext.Database.BeginTransactionAsync().ConfigureAwait(false);

        await _DbContext.Database
            .ExecuteSqlAsync(
                $"""
                select 1
                from level_locks
                where level_id = {levelId}
                for update
                """)
            .ConfigureAwait(false);

        await action().ConfigureAwait(false);

        await transaction.CommitAsync().ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteSequentialForLevelAsync<TResult>(long levelId, Func<Task<TResult>> action)
    {
        await using IDbContextTransaction transaction = await _DbContext.Database.BeginTransactionAsync().ConfigureAwait(false);

        await _DbContext.Database
            .ExecuteSqlAsync(
                $"""
                 select 1
                 from level_locks
                 where level_id = {levelId}
                 for update
                 """)
            .ConfigureAwait(false);

        TResult result = await action().ConfigureAwait(false);

        await transaction.CommitAsync().ConfigureAwait(false);

        return result;
    }
}