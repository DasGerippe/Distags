using Distags.Contracts;
using System.Text.Json.Serialization;

namespace Distags.Api.Contracts.Levels;

public class WorkshopLevelResponse : LevelResponse
{
    [JsonIgnore]
    public override LevelType Type => LevelType.Workshop;

    public long WorkshopId { get; set; }

    public long CreatorId { get; set; }

    public string? CreatorName { get; set; }

    public string? CreatorAvatarHash { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}
