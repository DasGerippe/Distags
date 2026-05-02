using Distags.Core.Dtos;

namespace Distags.Core.Queries.Contributions;

public class ContributionsPage
{
    public List<ContributionDto> Contributions { get; set; } = [];

    public string? NextCursor { get; set; }
}