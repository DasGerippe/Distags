namespace Distags.Api.Contracts.Contributions;

public class RevertContributionsRequest
{
    public long SteamId { get; init; }

    public DateTimeOffset? RevertStartDate { get; init; }
}
