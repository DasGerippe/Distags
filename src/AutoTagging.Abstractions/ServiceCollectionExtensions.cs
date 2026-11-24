using AutoTagging.Abstractions.Progress;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoTagging.Abstractions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProgressiveStrategy<TProgressStrategy, TTagDeltaCalculator>(
        this IServiceCollection services, string stateKey)
        where TProgressStrategy : class, IProgressStrategy
        where TTagDeltaCalculator : class, ITagDeltaCalculator
    {
        services.TryAddTransient<TProgressStrategy>();
        services.TryAddTransient<TTagDeltaCalculator>();
        services.AddTransient<ITaggingStrategy, ProgressiveTaggingStrategy<TProgressStrategy, TTagDeltaCalculator>>(serviceProvider =>
            ActivatorUtilities.CreateInstance<ProgressiveTaggingStrategy<TProgressStrategy, TTagDeltaCalculator>>(serviceProvider, stateKey));

        return services;
    }
}
