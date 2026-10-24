namespace Distags.Api.Contracts.Contributions;

public class ContributionAutoTaggerResponse
{
    public required Guid Guid { get; init; }

    public required string Name { get; init; }
}
