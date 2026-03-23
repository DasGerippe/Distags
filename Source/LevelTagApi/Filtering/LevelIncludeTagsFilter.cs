using DataClasses;

namespace LevelTagApi.Filtering;

public class LevelIncludeTagsFilter : IQueryableFilter<Level>
{
    private readonly IReadOnlyCollection<string> _FilterTagNames;

    public LevelIncludeTagsFilter(IReadOnlyCollection<string> filterTagNames)
    {
        _FilterTagNames = filterTagNames;
    }

    public IQueryable<Level> Filter(IQueryable<Level> source)
    {
        return source.Where(level => _FilterTagNames.All(tagName => level.Tags.Any(tag => tag.Name == tagName)));
    }
}