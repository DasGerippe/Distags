using Distags.Contracts;

namespace Distags.Api.Contracts.Levels;

public class OfficialLevelResponse : LevelResponse
{
    public override LevelType Type => LevelType.Official;

    public string Slug { get; set; } = string.Empty;

    public string CreatorName { get; set; } = string.Empty;
}
