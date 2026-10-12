using Distags.Api.Contracts.Contributions;
using Distags.Api.Hubs;
using Distags.Api.Mapping;
using Distags.Core.Events;
using Microsoft.AspNetCore.SignalR;

namespace Distags.Api.EventHandlers;

public class ContributionCreatedBroadcastingEventHandler : IAppEventHandler<ContributionCreatedEvent>
{
    private readonly IHubContext<ContributionHub, IContributionHubClient> _ContributionHub;

    public ContributionCreatedBroadcastingEventHandler(IHubContext<ContributionHub, IContributionHubClient> contributionHub)
    {
        _ContributionHub = contributionHub;
    }

    public async Task HandleAsync(ContributionCreatedEvent appEvent)
    {
        ContributionResponse contributionForModerators = ContributionMapper.Map(appEvent.Contribution);
        ContributionResponse contributionForUsers = ContributionMapper.Map(appEvent.AnonymizedContribution);

        await _ContributionHub.Clients.Group(ContributionHub.ModeratorsGroupName).NewContribution(contributionForModerators);
        await _ContributionHub.Clients.Group(ContributionHub.UsersGroupName).NewContribution(contributionForUsers);
    }
}
