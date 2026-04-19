using System.ComponentModel.DataAnnotations;

namespace LevelTagApi.Dtos.Contributions;

public class GetContributionsParameters
{
    public long? UserSteamId { get; set; }

    public long? LevelId { get; set; }

    public string? TagName { get; set; }

    [Range(1, 100, ErrorMessage = $"{nameof(Limit)} must be between 1 and 100.")]
    public int Limit { get; set; } = 100;

    public string? Cursor { get; set; }
}