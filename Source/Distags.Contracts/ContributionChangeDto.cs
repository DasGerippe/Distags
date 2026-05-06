namespace Distags.Contracts;

public class ContributionChangeDto
{
    public ChangeActionDto Action { get; set; }

    public string TagName { get; set; } = string.Empty;
}