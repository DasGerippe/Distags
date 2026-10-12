using Distags.WebApp.Components.Inputs;

namespace Distags.WebApp.Services;

public class IntDropdownItemProvider : IGenericDropdownItemProvider<int>
{
    public IReadOnlyDictionary<int, string> Items { get; }

    public IntDropdownItemProvider(params int[] integers)
    {
        Items = integers.ToDictionary(integer => integer, integer => integer.ToString());
    }
}
