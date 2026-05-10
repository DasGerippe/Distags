using AutoTagging.Abstractions;
using Distags.Core;
using Distags.Core.Mapping;

namespace AutoTaggerRunner;

public class AutoTaggerLevelProviderFactory : IAutoTaggerLevelProviderFactory
{
    private readonly IRequestDispatcher _RequestDispatcher;

    public AutoTaggerLevelProviderFactory(IRequestDispatcher requestDispatcher)
    {
        _RequestDispatcher = requestDispatcher;
    }

    public IAutoTaggerLevelProvider CreateLevelProvider(TaggingStrategy taggingStrategy, LevelProjectionIncludes requiredIncludes)
    {
        return new AutoTaggerLevelProvider(taggingStrategy, requiredIncludes, _RequestDispatcher);
    }
}