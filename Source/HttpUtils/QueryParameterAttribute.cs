namespace HttpUtils;

[AttributeUsage(AttributeTargets.Property)]
public class QueryParameterAttribute : Attribute
{
    public string ParameterKey { get; }

    public QueryParameterAttribute(string parameterKey)
    {
        ParameterKey = parameterKey;
    }
}