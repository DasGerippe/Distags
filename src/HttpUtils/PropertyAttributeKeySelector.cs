using System.Reflection;

namespace HttpUtils;

public class PropertyAttributeKeySelector : IPropertyKeySelector
{
    public string? GetKey(PropertyInfo property)
    {
        HttpParameterAttribute? httpParameterAttribute = property.GetCustomAttribute<HttpParameterAttribute>();
        return httpParameterAttribute?.ParameterKey;
    }
}
