using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.CreateOrUpdateAutoTagger;
using Distags.Core.Queries.AutoTaggerByGuid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoTaggerWorker;

public class AutoTaggerScheduler : BackgroundService
{
    private readonly AutoTaggerQueue _AutoTaggerQueue;
    private readonly IServiceScopeFactory _ScopeFactory;
    private readonly ILogger<AutoTaggerScheduler> _Logger;

    private Dictionary<Type, TimeSpan> _AutoTaggerIntervals = [];

    private Dictionary<Type, DateTimeOffset> _AutoTaggerDueDates = [];

    public AutoTaggerScheduler(
        AutoTaggerQueue autoTaggerQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<AutoTaggerScheduler> logger)
    {
        _AutoTaggerQueue = autoTaggerQueue;
        _ScopeFactory = scopeFactory;
        _Logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _ScopeFactory.CreateScope();

        IRequestDispatcher requestDispatcher = scope.ServiceProvider.GetRequiredService<IRequestDispatcher>();
        List<AutoTagger> autoTaggers = scope.ServiceProvider.GetServices<AutoTagger>().ToList();

        foreach (AutoTagger autoTagger in autoTaggers)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            CreateOrUpdateAutoTaggerCommand command = new CreateOrUpdateAutoTaggerCommand
            {
                Guid = autoTagger.Guid,
                Name = autoTagger.Name,
            };
            await requestDispatcher.DispatchAsync(command);

            if (autoTagger.Interval == null)
                continue;

            AutoTaggerByGuidQuery query = new AutoTaggerByGuidQuery { AutoTaggerGuid = autoTagger.Guid };
            AutoTaggerDto? autoTaggerDto = await requestDispatcher.DispatchAsync(query);

            Type autoTaggerType = autoTagger.GetType();

            _AutoTaggerIntervals[autoTaggerType] = autoTagger.Interval.Value;
            _AutoTaggerDueDates[autoTaggerType] = autoTaggerDto?.LastRunOn + autoTagger.Interval.Value ?? DateTimeOffset.UtcNow;
        }

        _Logger.LogInformation("Scheduling {count} AutoTaggers. ({autoTaggers})",
            _AutoTaggerIntervals.Count, string.Join(", ", _AutoTaggerIntervals.Keys.Select(type => type.Name)));

        await base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _AutoTaggerIntervals = [];
        _AutoTaggerDueDates = [];

        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTimeOffset utcNow = DateTimeOffset.UtcNow;
                List<Type> dueAutoTaggerTypes = GetDueAutoTaggerTypes(utcNow);

                foreach (Type autoTaggerType in dueAutoTaggerTypes)
                {
                    _AutoTaggerQueue.TryEnqueue(autoTaggerType);
                    _AutoTaggerDueDates[autoTaggerType] = utcNow + _AutoTaggerIntervals[autoTaggerType];
                }

                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private List<Type> GetDueAutoTaggerTypes(DateTimeOffset utcNow)
    {
        return _AutoTaggerDueDates
            .Where(autoTaggerDueDate => autoTaggerDueDate.Value <= utcNow)
            .Select(autoTaggerDueDate => autoTaggerDueDate.Key)
            .ToList();
    }
}
