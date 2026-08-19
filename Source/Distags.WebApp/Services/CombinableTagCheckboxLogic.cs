using Distags.WebApp.Models;

namespace Distags.WebApp.Services;

public class CombinableTagCheckboxLogic : ICheckboxLogic<TagFilterState>
{
    public TagFilterState GetNextState(TagFilterState currentState)
    {
        return currentState switch
        {
            TagFilterState.Neutral => TagFilterState.Required,
            TagFilterState.Required => TagFilterState.Forbidden,
            TagFilterState.Forbidden => TagFilterState.Neutral,
            _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null),
        };
    }
}
