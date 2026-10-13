using Distags.Contracts;
using System.Text.Json.Serialization;

namespace Distags.Api.Contracts.Levels;

public class WorkshopLevelResponse : LevelResponse
{
    [JsonIgnore]
    public override LevelType Type => LevelType.Workshop;

    public long WorkshopId { get; init; }

    public long CreatorId { get; init; }

    public string? CreatorName { get; init; }

    public string? CreatorAvatarHash { get; init; }

    public string ImageUrl { get; init; } = string.Empty;

    public DateTimeOffset CreatedOn { get; init; }

    public DateTimeOffset UpdatedOn { get; init; }
}
