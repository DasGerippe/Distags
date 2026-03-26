using System.Text.Json;
using System.Text.Json.Serialization;

namespace LevelTagApi.Dtos;

public class PatchValueConverter<TValue> : JsonConverter<PatchValue<TValue>>
{
    public override bool HandleNull => true;

    public override PatchValue<TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        TValue? value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        return new PatchValue<TValue> { Value = value };
    }

    public override void Write(Utf8JsonWriter writer, PatchValue<TValue> patchValue, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, patchValue.Value, options);
    }
}