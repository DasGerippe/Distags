namespace LevelTagApi.Dtos;

public class GetLevelsResponse
{
    public int TotalLevels { get; set; }

    public List<GetLevelDto> Levels { get; set; } = [];
}