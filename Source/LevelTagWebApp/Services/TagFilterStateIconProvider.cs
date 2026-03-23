using LevelTagWebApp.Models;

namespace LevelTagWebApp.Services;

public class TagFilterStateIconProvider : ICheckboxIconProvider<TagFilterState>
{
    public string GetIconId(TagFilterState currentState)
    {
        return currentState switch
        {
            TagFilterState.Neutral => "square-regular",
            TagFilterState.Required => "square-check-solid",
            TagFilterState.Forbidden => "square-xmark-solid",
            _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null),
        };
    }

    public string GetIconClass(TagFilterState currentState)
    {
        return currentState switch
        {
            TagFilterState.Neutral => "text-muted",
            TagFilterState.Required => "text-success",
            TagFilterState.Forbidden => "text-danger",
            _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null),
        };
    }
}