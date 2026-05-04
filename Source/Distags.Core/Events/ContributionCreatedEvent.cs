namespace Distags.Core.Events;

public class ContributionCreatedEvent : IAppEvent
{
    public long ContributionId { get; }

    public ContributionCreatedEvent(long contributionId)
    {
        ContributionId = contributionId;
    }
}