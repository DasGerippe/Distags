namespace LevelTagApi.Dtos.Contributions;

public class GetContributionsResponse
{
    public List<ContributionDto> Contributions { get; set; } = [];

    public string? NextCursor { get; set; } = string.Empty;
}