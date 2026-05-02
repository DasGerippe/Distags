namespace Distags.Core.Queries.Contributions;

public sealed record ContributionsCursor
{
    public DateTimeOffset ContributedOn { get; set; }
}