using System.ComponentModel.DataAnnotations;

namespace Distags.Api.Contracts.Levels;

public class BrowseLevelsRequest
{
    public string? LevelName { get; init; }

    public string? CreatorName { get; init; }

    public string? IncludeTags { get; init; }

    public string? ExcludeTags { get; init; }

    public LevelSortStrategy SortStrategy { get; init; } = LevelSortStrategy.CreatedDesc;

    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; init; } = 25;
}
