using LevelTagWebApp.Models;

namespace LevelTagWebApp.Services;

public class ExclusiveTagCheckboxLogic : ICheckboxLogic<TagFilterState>
{
    public TagFilterState GetNextState(TagFilterState currentState)
    {
        return currentState == TagFilterState.Neutral
            ? TagFilterState.Forbidden
            : TagFilterState.Neutral;
    }
}