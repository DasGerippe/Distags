namespace LevelTagApi.Dtos.Contribution;

public class ChangeDto
{
    public TagAction Action { get; set; }

    public string TagName { get; set; } = string.Empty;
}