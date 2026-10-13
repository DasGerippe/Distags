using System.Reflection;

namespace HttpUtils;

public class PropertyAttributeKeySelector : IPropertyKeySelector
{
    public string? GetKey(PropertyInfo property)
    {
        QueryParameterAttribute? queryParameterAttribute = property.GetCustomAttribute<QueryParameterAttribute>();
        return queryParameterAttribute?.ParameterKey;
    }
}
