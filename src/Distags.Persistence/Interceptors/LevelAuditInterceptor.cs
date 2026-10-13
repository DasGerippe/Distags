using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Distags.Persistence.Interceptors;

public class LevelAuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetLevelTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = new CancellationToken())
    {
        SetLevelTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetLevelTimestamps(DbContext? dbContext)
    {
        if (dbContext == null)
            return;

        DateTimeOffset utcNow = DateTime.UtcNow;

        foreach (EntityEntry<Level> levelEntry in dbContext.ChangeTracker.Entries<Level>())
        {
            if (levelEntry.State == EntityState.Added)
            {
                levelEntry.Entity.RecordCreatedOn = utcNow;
                levelEntry.Entity.RecordUpdatedOn = utcNow;
            }

            if (levelEntry.State == EntityState.Modified)
                levelEntry.Entity.RecordUpdatedOn = utcNow;
        }
    }
}
