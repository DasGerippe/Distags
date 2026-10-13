namespace Distags.WebApp.Services;

public interface ICheckboxLogic<TState>
{
    TState GetNextState(TState currentState);
}
