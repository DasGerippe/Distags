namespace LevelTagApi.Dtos.Contributions;

public class ChangeDto
{
    public TagAction Action { get; set; }

    public string TagName { get; set; } = string.Empty;
}