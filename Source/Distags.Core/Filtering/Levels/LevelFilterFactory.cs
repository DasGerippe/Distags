using Distags.Core.Entities;

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

        if (filterParams.LevelName != null)
            levelFilterGroup.AddFilter(
                _StringContainsFilterFactory.CreateFilter<Level>(level => level.Name, filterParams.LevelName));

        if (filterParams.CreatorName != null)
            levelFilterGroup.AddFilter(
                _StringContainsFilterFactory.CreateFilter<Level>(level => level.Creator!.Name, filterParams.CreatorName));

        if (filterParams.CreatedAfter != null)
            levelFilterGroup.AddFilter(new SimpleFilter<Level>(level => level.CreatedOn >= filterParams.CreatedAfter));

        if (filterParams.UpdatedAfter != null)
            levelFilterGroup.AddFilter(new SimpleFilter<Level>(level => level.UpdatedOn >= filterParams.UpdatedAfter));

        if (filterParams.IncludeTags.Count > 0)
            levelFilterGroup.AddFilter(new LevelIncludeTagsFilter(filterParams.IncludeTags));

        if (filterParams.ExcludeTags.Count > 0)
            levelFilterGroup.AddFilter(new LevelExcludeTagsFilter(filterParams.ExcludeTags));

        return levelFilterGroup;
    }
}
