namespace Distags.Core.Dtos;

public class ContributionChangeDto
{
    public ChangeActionDto Action { get; set; }

    public string TagName { get; set; } = string.Empty;
}