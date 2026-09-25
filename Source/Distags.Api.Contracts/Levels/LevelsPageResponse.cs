namespace Distags.Api.Contracts.Levels;

public class LevelsPageResponse
{
    public List<LevelResponse> Levels { get; set; } = [];

    public int PageSize { get; set; }

    public int PageNumber { get; set; }

    public int TotalPages { get; set; }

    public int TotalLevels { get; set; }
}
