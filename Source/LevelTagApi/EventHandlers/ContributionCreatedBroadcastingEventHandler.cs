using Distags.Core.Dtos;
using Distags.Core.Events;
using Distags.Core.Queries;
using Distags.Core.Queries.ContributionById;
using LevelTagApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LevelTagApi.EventHandlers;

public class ContributionCreatedBroadcastingEventHandler : IAppEventHandler<ContributionCreatedEvent>
{
    private readonly IHubContext<ContributionHub, IContributionHubClient> _ContributionHub;
    private readonly IQueryHandler<ContributionByIdQuery, ContributionDto?> _ContributionByIdQueryHandler;

    public ContributionCreatedBroadcastingEventHandler(
        IHubContext<ContributionHub, IContributionHubClient> contributionHub,
        IQueryHandler<ContributionByIdQuery, ContributionDto?> contributionByIdQueryHandler)
    {
        _ContributionHub = contributionHub;
        _ContributionByIdQueryHandler = contributionByIdQueryHandler;
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
        ContributionDto? contributionDto = await _ContributionByIdQueryHandler.HandleAsync(query);
        if (contributionDto != null)
            await _ContributionHub.Clients.Group(groupName).NewContribution(contributionDto);
    }
}