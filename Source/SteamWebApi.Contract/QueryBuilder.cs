using System.Collections;
using SteamWebApi.Dtos;
using System.Reflection;
using System.Text;

namespace SteamWebApi;

public class QueryBuilder
{
    public string BuildQueryString(object parameters)
    {
        StringBuilder queryStringBuilder = new StringBuilder();

        foreach (PropertyInfo property in parameters.GetType().GetProperties())
        {
            QueryParameterAttribute? queryParameterAttribute = property.GetCustomAttribute<QueryParameterAttribute>();
            if (queryParameterAttribute == null)
                continue;

            object? value = property.GetValue(parameters);
            if (value == null)
                continue;

            string key = queryParameterAttribute.ParameterKey;
            switch (value)
            {
                // Check for string before IEnumerable because string implements IEnumerable.
                case string stringValue:
                    value = Uri.EscapeDataString(stringValue);
                    break;

                case Enum enumValue:
                    value = Convert.ChangeType(value, enumValue.GetType().GetEnumUnderlyingType());
                    break;

                case IEnumerable enumerable:
                    AppendKeyValueList(queryStringBuilder, key, enumerable);
                    continue;
            }

            AppendKeyValuePair(queryStringBuilder, key, value);
        }

        return queryStringBuilder.ToString();
    }

    private void AppendKeyValuePair(StringBuilder queryStringBuilder, string key, object value)
    {
        AppendKey(queryStringBuilder, key);
        queryStringBuilder.Append(value);
    }

    private void AppendKeyValueList(StringBuilder queryStringBuilder, string key, IEnumerable values)
    {
        AppendKey(queryStringBuilder, key);
        AppendValueList(queryStringBuilder, values);
    }

    private void AppendKey(StringBuilder queryStringBuilder, string key)
    {
        if (queryStringBuilder.Length > 0)
            queryStringBuilder.Append('&');

        queryStringBuilder.Append(key);
        queryStringBuilder.Append('=');
    }

    private void AppendValueList(StringBuilder queryStringBuilder, IEnumerable values)
    {
        bool appendComma = false;
        foreach (object value in values)
        {
            if (appendComma)
                queryStringBuilder.Append(',');

            queryStringBuilder.Append(value);
            appendComma = true;
        }
    }
}