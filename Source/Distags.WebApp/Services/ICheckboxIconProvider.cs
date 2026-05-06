namespace Distags.WebApp.Services;

public interface ICheckboxIconProvider<TState>
{
    string GetIconId(TState currentState);

    string GetIconClass(TState currentState);
}