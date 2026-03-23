using AutoTagging.Abstractions;
using DataStoring;
using Microsoft.EntityFrameworkCore;

namespace AutoTaggerRunner;

public class AutoTaggerLevelProviderFactory : IAutoTaggerLevelProviderFactory
{
    private readonly IDbContextFactory<LevelTagDbContext> _DbContextFactory;

    public AutoTaggerLevelProviderFactory(IDbContextFactory<LevelTagDbContext> dbContextFactory)
    {
        _DbContextFactory = dbContextFactory;
    }

    public IAutoTaggerLevelProvider CreateLevelProvider(TaggingStrategy taggingStrategy)
    {
        return taggingStrategy switch
        {
            TaggingStrategy.NewLevels => new AutoTaggerNewLevelsProvider(_DbContextFactory),
            TaggingStrategy.NewAndUpdatedLevels => new AutoTaggerUpdatedLevelsProvider(_DbContextFactory),
            _ => throw new ArgumentOutOfRangeException(nameof(taggingStrategy), taggingStrategy, null),
        };
    }
}