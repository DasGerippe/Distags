using Distags.Contracts;
using System.Text.Json.Serialization;

namespace Distags.Api.Contracts.Levels;

public class OfficialLevelResponse : LevelResponse
{
    [JsonIgnore]
    public override LevelType Type => LevelType.Official;

    public string Slug { get; init; } = string.Empty;

    public string CreatorName { get; init; } = string.Empty;
}
