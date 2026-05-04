namespace Distags.Core.Dtos;

public class TagChangeDto
{
    public TagChangeType ChangeType { get; set; }

    public string TagName { get; set; } = string.Empty;
}