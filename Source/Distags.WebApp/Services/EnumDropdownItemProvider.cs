using Distags.WebApp.Components.Inputs;
using Microsoft.Extensions.Localization;

namespace Distags.WebApp.Services;

public class EnumDropdownItemProvider<TEnum> : IGenericDropdownItemProvider<TEnum> where TEnum : struct, Enum
{
    public IReadOnlyDictionary<TEnum, string> Items { get; }

    public EnumDropdownItemProvider(IStringLocalizer localizer)
    {
        string enumTypeName = typeof(TEnum).Name;
        Items = Enum.GetValues<TEnum>().ToDictionary(
            enumValue => enumValue,
            enumValue => localizer[$"Enum_{enumTypeName}_{Enum.GetName(enumValue)}"].Value);
    }
}