using LevelTagWebApp.Filters;

namespace LevelTagWebApp.Services;

public class TagFilterCheckboxLogicProvider : ICheckboxLogicProvider<TagFilterState, FilterGroupType>
{
    private readonly ICheckboxLogic<TagFilterState> _ExclusiveTagFilterLogic = new ExclusiveTagCheckboxLogic();
    private readonly ICheckboxLogic<TagFilterState> _CombinableTagFilterLogic = new CombinableTagCheckboxLogic();

    public ICheckboxLogic<TagFilterState> GetCheckboxLogic(FilterGroupType filterGroupType)
    {
        return filterGroupType switch
        {
            FilterGroupType.ExclusiveTags => _ExclusiveTagFilterLogic,
            FilterGroupType.CombinableTags => _CombinableTagFilterLogic,
            _ => throw new ArgumentOutOfRangeException(nameof(filterGroupType), filterGroupType, null),
        };
    }
}