using Distags.Api.Hubs;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Events;
using Distags.Core.Queries.ContributionById;
using Microsoft.AspNetCore.SignalR;

namespace Distags.Api.EventHandlers;

public class ContributionCreatedBroadcastingEventHandler : IAppEventHandler<ContributionCreatedEvent>
{
    private readonly IHubContext<ContributionHub, IContributionHubClient> _ContributionHub;
    private readonly IRequestDispatcher _RequestDispatcher;

    public ContributionCreatedBroadcastingEventHandler(
        IHubContext<ContributionHub, IContributionHubClient> contributionHub,
        IRequestDispatcher requestDispatcher)
    {
        _ContributionHub = contributionHub;
        _RequestDispatcher = requestDispatcher;
    }

    public async Task HandleAsync(ContributionCreatedEvent appEvent)
    {
        await BroadcastToGroup(appEvent.ContributionId, ContributionHub.ModeratorsGroupName, true);
        await BroadcastToGroup(appEvent.ContributionId, ContributionHub.UsersGroupName, false);
    }

    private async Task BroadcastToGroup(long contributionId, string groupName, bool ignoreAnonymity)
    {
        ContributionByIdQuery query = new ContributionByIdQuery
        {
            ContributionId = contributionId,
            IgnoreAnonymity = ignoreAnonymity,
        };
        ContributionDto? contributionDto = await _RequestDispatcher.DispatchAsync(query);
        if (contributionDto != null)
            await _ContributionHub.Clients.Group(groupName).NewContribution(contributionDto);
    }
}
