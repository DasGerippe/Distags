namespace LevelTagWebApp.Services;

public interface ICheckboxLogic<TState>
{
    TState GetNextState(TState currentState);
}