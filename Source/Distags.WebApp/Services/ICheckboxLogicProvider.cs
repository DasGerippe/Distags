namespace Distags.WebApp.Services;

public interface ICheckboxLogicProvider<TState, in TLogicDecider>
{
    ICheckboxLogic<TState> GetCheckboxLogic(TLogicDecider decider);
}