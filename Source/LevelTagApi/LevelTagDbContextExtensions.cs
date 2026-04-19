using DataStoring;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi;

public static class LevelTagDbContextExtensions
{
    public static Task LockLevelAsync(this LevelTagDbContext dbContext, long levelId)
    {
        return dbContext.Database.ExecuteSqlRawAsync(
            """
            select 1
            from level_locks
            where level_id = {0}
            for update
            """, levelId);
    }
}