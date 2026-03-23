using DataClasses;
using DataStoring;
using LevelTagApi.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Concurrent;

namespace LevelTagApi.Interceptors;

public class LevelTagBroadcastingInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<LevelTagBroadcastingInterceptor> _Logger;

    private readonly ConcurrentDictionary<DbContextId, LevelTagChanges> _CapturedChanges =
        new ConcurrentDictionary<DbContextId, LevelTagChanges>();

    public LevelTagBroadcastingInterceptor(ILogger<LevelTagBroadcastingInterceptor> logger)
    {
        _Logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CaptureLevelTagsChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        CaptureLevelTagsChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ProcessCapturedLevelTagChangesAsync(eventData.Context).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await ProcessCapturedLevelTagChangesAsync(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        ClearCapturedChanges(eventData.Context);
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        ClearCapturedChanges(eventData.Context);
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override void SaveChangesCanceled(DbContextEventData eventData)
    {
        ClearCapturedChanges(eventData.Context);
        base.SaveChangesCanceled(eventData);
    }

    public override Task SaveChangesCanceledAsync(DbContextEventData eventData, CancellationToken cancellationToken = default)
    {
        ClearCapturedChanges(eventData.Context);
        return base.SaveChangesCanceledAsync(eventData, cancellationToken);
    }

    private void CaptureLevelTagsChanges(DbContext? dbContext)
    {
        if (dbContext == null)
            return;

        List<LevelTag> addedLevelTags = dbContext.ChangeTracker.Entries<LevelTag>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();

        List<LevelTag> deletedLevelTags = dbContext.ChangeTracker.Entries<LevelTag>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Select(entry => entry.Entity)
            .ToList();

        if (addedLevelTags.Count == 0 && deletedLevelTags.Count == 0)
            return;

        LevelTagChanges levelTagChanges = new LevelTagChanges
        {
            AddedLevelTags = addedLevelTags,
            RemovedLevelTags = deletedLevelTags,
        };

        _CapturedChanges.AddOrUpdate(dbContext.ContextId, levelTagChanges, (_, _) => levelTagChanges);
    }

    private async Task ProcessCapturedLevelTagChangesAsync(DbContext? dbContext)
    {
        if (dbContext == null)
            return;

        if (!_CapturedChanges.TryRemove(dbContext.ContextId, out LevelTagChanges? changes))
            return;

        try
        {
            ILevelTagBroadcaster levelTagBroadcaster = dbContext.GetService<ILevelTagBroadcaster>();
            await levelTagBroadcaster.BroadcastLevelTagChangesAsync(changes.AddedLevelTags, changes.RemovedLevelTags);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while broadcasting level tag changes.");
        }
    }

    private void ClearCapturedChanges(DbContext? dbContext)
    {
        if (dbContext is LevelTagDbContext)
            _CapturedChanges.TryRemove(dbContext.ContextId, out _);
    }

    private class LevelTagChanges
    {
        public required List<LevelTag> AddedLevelTags { get; init; }

        public required List<LevelTag> RemovedLevelTags { get; init; }
    }
}