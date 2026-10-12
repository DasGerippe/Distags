using Distags.Contracts;

namespace Distags.Core.Queries.ContributionById;

public class ContributionByIdQuery : IQuery<ContributionDto?>
{
    public long ContributionId { get; set; }

    public bool IgnoreAnonymity { get; set; }
}
