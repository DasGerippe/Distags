using Distags.Contracts;
using System.Text.Json.Serialization;

namespace Distags.Api.Contracts.Levels;

public class OfficialLevelResponse : LevelResponse
{
    [JsonIgnore]
    public override LevelType Type => LevelType.Official;

    public string Slug { get; set; } = string.Empty;

    public string CreatorName { get; set; } = string.Empty;
}
