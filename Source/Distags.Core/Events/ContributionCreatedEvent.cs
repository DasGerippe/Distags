using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Mapping;

namespace Distags.Core.Events;

public class ContributionCreatedEvent : IAppEvent
{
    public ContributionDto Contribution { get; }

    public ContributionDto AnonymizedContribution { get; }

    public ContributionCreatedEvent(Contribution contribution)
    {
        Contribution = ContributionMapper.Map(contribution);
        AnonymizedContribution = ContributionAnonymizedMapper.Map(contribution);
    }
}
