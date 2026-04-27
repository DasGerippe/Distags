using Distags.Core.Dtos;

namespace Distags.Core.Queries.BrowseLevels;

public class LevelsPage
{
    public List<LevelDto> Levels { get; set; } = [];

    public int PageSize { get; set; }

    public int PageNumber { get; set; }

    public int TotalPages { get; set; }

    public int TotalLevels { get; set; }
}