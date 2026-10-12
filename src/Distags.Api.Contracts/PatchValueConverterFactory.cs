using System.Text.Json;
using System.Text.Json.Serialization;

namespace Distags.Api.Contracts;

public class PatchValueConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(PatchValue<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(PatchValueConverter<>).MakeGenericType(valueType);
        return Activator.CreateInstance(converterType) as JsonConverter;
    }
}
