namespace Distags.Contracts;

public class TagChangeDto
{
    public TagChangeAction Action { get; init; }

    public string TagName { get; init; } = string.Empty;
}
