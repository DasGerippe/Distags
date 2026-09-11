namespace Distags.Api.Contracts.Contributions;

public class RevertContributionsRequest
{
    public long SteamId { get; set; }

    public DateTimeOffset? RevertStartDate { get; set; }
}
