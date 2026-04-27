using DataClasses;

namespace Distags.Core.Filtering.Levels;

public class LevelFilterFactory : IQueryableFilterFactory<Level, LevelFilterParameters>
{
    private readonly IStringContainsFilterFactory _StringContainsFilterFactory;

    public LevelFilterFactory(IStringContainsFilterFactory stringContainsFilterFactory)
    {
        _StringContainsFilterFactory = stringContainsFilterFactory;
    }

    public IQueryableFilter<Level> GetQueryableFilter(LevelFilterParameters filterParams)
    {
        FilterGroup<Level> levelFilterGroup = new FilterGroup<Level>();

        if (filterParams.LevelFilter != null)
            levelFilterGroup.AddFilter(
                _StringContainsFilterFactory.CreateFilter<Level>(level => level.Name, filterParams.LevelFilter));

        if (filterParams.CreatorFilter != null)
            levelFilterGroup.AddFilter(
                _StringContainsFilterFactory.CreateFilter<Level>(level => level.Creator!.Name, filterParams.CreatorFilter));

        if (filterParams.IncludeTags.Count > 0)
            levelFilterGroup.AddFilter(new LevelIncludeTagsFilter(filterParams.IncludeTags));

        if (filterParams.ExcludeTags.Count > 0)
            levelFilterGroup.AddFilter(new LevelExcludeTagsFilter(filterParams.ExcludeTags));

        return levelFilterGroup;
    }
}