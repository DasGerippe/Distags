using DataClasses;
using LevelTagApi.Dtos;

namespace LevelTagApi.Filtering;

public class BrowseLevelsFilterFactory : IQueryableFilterFactory<Level, BrowseLevelsParameters>
{
    public IQueryableFilter<Level> GetQueryableFilter(BrowseLevelsParameters parameters)
    {
        FilterGroup<Level> levelFilterGroup = new FilterGroup<Level>();

        if (parameters.LevelFilter != null)
            levelFilterGroup.AddFilter(new StringFilter<Level>(level => level.Name, parameters.LevelFilter));

        if (parameters.CreatorFilter != null)
            levelFilterGroup.AddFilter(new StringFilter<Level>(level => level.Creator!.Name, parameters.CreatorFilter));

        if (parameters.IncludeTags != null)
        {
            List<string> tagNames = GetNormalizedTagNames(parameters.IncludeTags);
            levelFilterGroup.AddFilter(new LevelIncludeTagsFilter(tagNames));
        }

        if (parameters.ExcludeTags != null)
        {
            List<string> tagNames = GetNormalizedTagNames(parameters.ExcludeTags);
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