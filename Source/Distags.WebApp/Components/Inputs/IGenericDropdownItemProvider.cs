namespace Distags.WebApp.Components.Inputs;

public interface IGenericDropdownItemProvider<TValue>
{
    IReadOnlyDictionary<TValue, string> Items { get; }
}
