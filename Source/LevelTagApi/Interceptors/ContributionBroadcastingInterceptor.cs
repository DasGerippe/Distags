using System.Collections.Concurrent;
using DataClasses;
using DataStoring;
using LevelTagApi.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LevelTagApi.Interceptors;

public class ContributionBroadcastingInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<ContributionBroadcastingInterceptor> _Logger;

    private readonly ConcurrentDictionary<DbContextId, List<Contribution>> _AddedContributions =
        new ConcurrentDictionary<DbContextId, List<Contribution>>();

    public ContributionBroadcastingInterceptor(ILogger<ContributionBroadcastingInterceptor> logger)
    {
        _Logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CaptureNewContributions(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        CaptureNewContributions(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ProcessStoredContributionsAsync(eventData.Context).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await ProcessStoredContributionsAsync(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesCanceled(DbContextEventData eventData)
    {
        ClearAddedContributions(eventData.Context);
        base.SaveChangesCanceled(eventData);
    }

    public override Task SaveChangesCanceledAsync(
        DbContextEventData eventData, CancellationToken cancellationToken = default)
    {
        ClearAddedContributions(eventData.Context);
        return base.SaveChangesCanceledAsync(eventData, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        ClearAddedContributions(eventData.Context);
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        ClearAddedContributions(eventData.Context);
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void CaptureNewContributions(DbContext? dbContext)
    {
        if (dbContext is not LevelTagDbContext)
            return;

        List<Contribution> newContributions = dbContext.ChangeTracker.Entries<Contribution>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();

        if (newContributions.Count > 0)
            _AddedContributions.AddOrUpdate(dbContext.ContextId, newContributions, (_, _) => newContributions);
    }

    private async Task ProcessStoredContributionsAsync(DbContext? dbContext)
    {
        if (dbContext is not LevelTagDbContext)
            return;

        if (!_AddedContributions.TryRemove(dbContext.ContextId, out List<Contribution>? storedContributions))
            return;

        try
        {
            IContributionBroadcaster contributionsHubServer = dbContext.GetService<IContributionBroadcaster>();
            await contributionsHubServer.BroadcastNewContributionsAsync(storedContributions);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while broadcasting new contributions after changes were saved.");
        }
    }

    private void ClearAddedContributions(DbContext? dbContext)
    {
        if (dbContext != null)
            _AddedContributions.TryRemove(dbContext.ContextId, out List<Contribution>? _);
    }
}