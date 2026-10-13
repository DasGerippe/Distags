using System.Reflection;

namespace HttpUtils;

public class PropertyNameKeySelector : IPropertyKeySelector
{
    public string? GetKey(PropertyInfo property)
    {
        return property.Name;
    }
}
