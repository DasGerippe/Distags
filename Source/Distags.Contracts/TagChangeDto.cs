namespace Distags.Contracts;

public class TagChangeDto
{
    public TagChangeType ChangeType { get; init; }

    public string TagName { get; init; } = string.Empty;
}