using System.Collections;
using System.Reflection;
using System.Text;

namespace HttpUtils;

public static class QueryBuilder
{
    public static string BuildQueryString(object parameters)
    {
        return BuildQueryString(parameters, new PropertyNameKeySelector());
    }

    public static string BuildQueryString(object parameters, IPropertyKeySelector propertyKeySelector)
    {
        StringBuilder queryStringBuilder = new StringBuilder();

        foreach (PropertyInfo property in parameters.GetType().GetProperties())
        {
            string? key = propertyKeySelector.GetKey(property);
            if (key == null)
                continue;

            object? value = property.GetValue(parameters);
            if (value == null)
                continue;

            AppendKeyValuePair(queryStringBuilder, key, value);
        }

        return queryStringBuilder.ToString();
    }

    private static void AppendKeyValuePair(StringBuilder queryStringBuilder, string key, object value)
    {
        AppendKey(queryStringBuilder, key);
        AppendValue(queryStringBuilder, value);
    }

    private static void AppendKey(StringBuilder queryStringBuilder, string key)
    {
        if (queryStringBuilder.Length > 0)
            queryStringBuilder.Append('&');

        queryStringBuilder.Append(key);
        queryStringBuilder.Append('=');
    }

    private static void AppendValue(StringBuilder queryStringBuilder, object value)
    {
        switch (value)
        {
            // Check for string before IEnumerable because string implements IEnumerable.
            case string stringValue:
                value = Uri.EscapeDataString(stringValue);
                queryStringBuilder.Append(value);
                break;

            case Enum enumValue:
                value = Convert.ChangeType(value, enumValue.GetType().GetEnumUnderlyingType());
                queryStringBuilder.Append(value);
                break;

            case IEnumerable enumerable:
                AppendValueList(queryStringBuilder, enumerable);
                break;

            case DateTimeOffset dateTimeOffset:
                value = Uri.EscapeDataString(dateTimeOffset.ToString("O"));
                queryStringBuilder.Append(value);
                break;

            default:
                queryStringBuilder.Append(value);
                break;
        }
    }

    private static void AppendValueList(StringBuilder queryStringBuilder, IEnumerable values)
    {
        bool appendComma = false;
        foreach (object value in values)
        {
            if (appendComma)
                queryStringBuilder.Append(',');

            AppendValue(queryStringBuilder, value);
            appendComma = true;
        }
    }
}
