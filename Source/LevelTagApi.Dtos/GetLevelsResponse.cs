namespace LevelTagApi.Dtos;

public class GetLevelsResponse
{
    public int PageSize { get; set; }

    public int PageNumber { get; set; }

    public int TotalLevels { get; set; }

    public int TotalPages { get; set; }

    public List<GetLevelDto> Levels { get; set; } = [];
}