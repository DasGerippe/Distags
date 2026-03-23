using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Hubs;

public class ContributionBroadcaster : IContributionBroadcaster
{
    private readonly IHubContext<ContributionHub, IContributionHubClient> _HubContext;
    private readonly LevelTagDbContext _DbContext;
    private readonly ContributionDtoConverter _DtoConverter;

    public ContributionBroadcaster(
        IHubContext<ContributionHub, IContributionHubClient> hubContext,
        LevelTagDbContext dbContext,
        ContributionDtoConverter dtoConverter)
    {
        _HubContext = hubContext;
        _DbContext = dbContext;
        _DtoConverter = dtoConverter;
    }

    public async Task BroadcastNewContributionAsync(Contribution contribution)
    {
        IQueryable<Contribution> contributionQuery = _DbContext.Set<Contribution>()
            .Where(dbContribution => dbContribution == contribution);

        Contribution reloadedContribution = await _DtoConverter.IncludeRequiredNavigations(contributionQuery).FirstAsync();

        ContributionDto dtoForUsers = _DtoConverter.ToDto(reloadedContribution, respectAnonymity: true);
        ContributionDto dtoForModerators = _DtoConverter.ToDto(reloadedContribution, respectAnonymity: false);

        Task userBroadcastTask = _HubContext.Clients.Group(ContributionHub.UsersGroupName).NewContribution(dtoForUsers);
        Task moderatorBroadcastTask = _HubContext.Clients.Group(ContributionHub.ModeratorsGroupName).NewContribution(dtoForModerators);

        await userBroadcastTask;
        await moderatorBroadcastTask;
    }

    public async Task BroadcastNewContributionsAsync(IEnumerable<Contribution> contributions)
    {
        if (contributions is ICollection<Contribution> { Count: 1 } collection)
        {
            await BroadcastNewContributionAsync(collection.First());
            return;
        }

        IEnumerable<long> contributionIds = contributions.Select(contribution => contribution.Id);

        IQueryable<Contribution> contributionQuery = _DbContext.Set<Contribution>()
            .Where(contribution => contributionIds.Contains(contribution.Id))
            .OrderBy(contribution => contribution.Id);

        List<ContributionDto> dtosForUsers = await _DtoConverter
            .SelectDto(contributionQuery, respectAnonymity: true)
            .ToListAsync();

        List<ContributionDto> dtosForModerators = await _DtoConverter
            .SelectDto(contributionQuery, respectAnonymity: true)
            .ToListAsync();

        List<Task> broadcastTasks = [];

        broadcastTasks.AddRange(dtosForUsers.Select(contributionDto =>
            _HubContext.Clients.Group(ContributionHub.UsersGroupName).NewContribution(contributionDto)));
        broadcastTasks.AddRange(dtosForModerators.Select(contributionDto =>
            _HubContext.Clients.Group(ContributionHub.ModeratorsGroupName).NewContribution(contributionDto)));

        await Task.WhenAll(broadcastTasks);
    }
}