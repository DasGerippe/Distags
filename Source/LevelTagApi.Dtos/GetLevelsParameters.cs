using System.ComponentModel.DataAnnotations;

namespace LevelTagApi.Dtos;

public class GetLevelsParameters
{
    public string? LevelFilter { get; set; }

    public string? CreatorFilter { get; set; }

    public string? IncludeTags { get; set; }

    public string? ExcludeTags { get; set; }

    public LevelSortStrategy SortStrategy { get; set; } = LevelSortStrategy.CreatedDesc;

    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 25;
}