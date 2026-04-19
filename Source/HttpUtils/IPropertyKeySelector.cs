using System.Reflection;

namespace HttpUtils;

public interface IPropertyKeySelector
{
    string? GetKey(PropertyInfo property);
}