namespace LevelTagWebApp.Filters.Abilities;

public class WingsFilterGroup : AbilityFilterGroup
{
    public WingsFilterGroup() : base("Wings", "wings")
    {
        foreach (TagFilter tagFilter in _TagFilters)
        {
            tagFilter.State = TagFilterState.Neutral;
        }
    }
}