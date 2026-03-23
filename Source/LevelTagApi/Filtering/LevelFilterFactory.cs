using DataClasses;

namespace LevelTagApi.Filtering;

public class LevelFilterFactory : IQueryableFilterFactory<Level, LevelFilterParameters>
{
    public IQueryableFilter<Level> GetQueryableFilter(LevelFilterParameters filterParams)
    {
        FilterGroup<Level> levelFilterGroup = new FilterGroup<Level>();

        if (filterParams.LevelFilter != null)
            levelFilterGroup.AddFilter(new StringFilter<Level>(level => level.Name, filterParams.LevelFilter));

        if (filterParams.CreatorFilter != null)
            levelFilterGroup.AddFilter(new StringFilter<Level>(level => level.Creator!.Name, filterParams.CreatorFilter));

        if (filterParams.IncludeTags != null)
        {
            List<string> tagNames = GetNormalizedTagNames(filterParams.IncludeTags);
            levelFilterGroup.AddFilter(new LevelIncludeTagsFilter(tagNames));
        }

        if (filterParams.ExcludeTags != null)
        {
            List<string> tagNames = GetNormalizedTagNames(filterParams.ExcludeTags);
            levelFilterGroup.AddFilter(new LevelExcludeTagsFilter(tagNames));
        }

        return levelFilterGroup;
    }

    private List<string> GetNormalizedTagNames(string commaSeparatedTags)
    {
        string[] tagNames = commaSeparatedTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        List<string> normalizedTagNames = tagNames.Select(tag => tag.ToLowerInvariant()).ToList();
        return normalizedTagNames;
    }
}