namespace Distags.Api.Contracts.Levels;

public class LevelsPageResponse
{
    public List<LevelResponse> Levels { get; init; } = [];

    public int PageSize { get; init; }

    public int PageNumber { get; init; }

    public int TotalPages { get; init; }

    public int TotalLevels { get; init; }
}
