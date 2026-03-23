using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamWebApi.Converter;

public class UnixDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        long unixTimestamp = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        long unixTimestamp = ((DateTimeOffset)value).ToUnixTimeSeconds();
        writer.WriteNumberValue(unixTimestamp);
    }
}