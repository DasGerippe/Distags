namespace HttpUtils;

[AttributeUsage(AttributeTargets.Property)]
public class HttpParameterAttribute : Attribute
{
    public string ParameterKey { get; }

    public HttpParameterAttribute(string parameterKey)
    {
        ParameterKey = parameterKey;
    }
}
