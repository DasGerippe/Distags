namespace Distags.Core.Entities;

public class ContributionChange
{
    public ChangeAction Action { get; set; }

    public string TagName { get; set; } = string.Empty;
}