using DataClasses;

namespace Distags.Core.Filtering.Levels;

public class LevelExcludeTagsFilter : IQueryableFilter<Level>
{
    private readonly IReadOnlyCollection<string> _FilterTagNames;

    public LevelExcludeTagsFilter(IReadOnlyCollection<string> filterTagNames)
    {
        _FilterTagNames = filterTagNames;
    }

    public IQueryable<Level> Filter(IQueryable<Level> source)
    {
        return source.Where(level => !_FilterTagNames.Any(tagName => level.Tags.Any(tag => tag.Name == tagName)));
    }
}