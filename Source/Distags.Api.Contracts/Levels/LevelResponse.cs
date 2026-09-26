using Distags.Contracts;
using System.Text.Json.Serialization;

namespace Distags.Api.Contracts.Levels;

[JsonDerivedType(typeof(OfficialLevelResponse), "official")]
[JsonDerivedType(typeof(WorkshopLevelResponse), "workshop")]
public abstract class LevelResponse
{
    [JsonIgnore]
    public abstract LevelType Type { get; }

    public string Name { get; init; } = string.Empty;

    public string Key { get; init; } = string.Empty;

    public List<string>? Tags { get; init; }
}
