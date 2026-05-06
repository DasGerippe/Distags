namespace Distags.Contracts;

public class ContributionsPage
{
    public List<ContributionDto> Contributions { get; set; } = [];

    public string? NextCursor { get; set; }
}