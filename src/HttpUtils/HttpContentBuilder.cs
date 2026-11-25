using System.Collections;
using System.Reflection;

namespace HttpUtils;

public static class HttpContentBuilder
{
    public static HttpContent BuildHttpContent(object parameters)
    {
        return BuildHttpContent(parameters, new PropertyNameKeySelector());
    }

    public static HttpContent BuildHttpContent(object parameters, IPropertyKeySelector propertyKeySelector)
    {
        Dictionary<string, string> keyValuePairs = [];

        foreach (PropertyInfo property in parameters.GetType().GetProperties())
        {
            string? key = propertyKeySelector.GetKey(property);
            if (key == null)
                continue;

            object? value = property.GetValue(parameters);
            if (value == null)
                continue;

            AddPropertyValue(keyValuePairs, key, value);
        }

        HttpContent content = new FormUrlEncodedContent(keyValuePairs);
        return content;
    }

    private static void AddPropertyValue(Dictionary<string, string> keyValuePairs, string key, object value)
    {
        switch (value)
        {
            // Check for string before IEnumerable because string implements IEnumerable.
            case string stringValue:
                keyValuePairs[key] = stringValue;
                break;

            case Enum enumValue:
                keyValuePairs[key] = Convert.ChangeType(value, enumValue.GetType().GetEnumUnderlyingType()).ToString()!;
                break;

            case IEnumerable enumerable:
                AddPropertyValueList(keyValuePairs, key, enumerable);
                break;

            case DateTimeOffset dateTimeOffset:
                keyValuePairs[key] = dateTimeOffset.ToString("O");
                break;

            default:
                string? objectString = value.ToString();
                if (objectString != null)
                    keyValuePairs[key] = objectString;
                break;
        }
    }

    private static void AddPropertyValueList(Dictionary<string, string> keyValuePairs, string key, IEnumerable values)
    {
        int index = 0;
        foreach (object value in values)
        {
            AddPropertyValue(keyValuePairs, $"{key}[{index++}]", value);
        }
    }
}
