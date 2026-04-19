using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos.Contributions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Hubs;

public class ContributionBroadcaster : IContributionBroadcaster
{
    private readonly IHubContext<ContributionHub, IContributionHubClient> _HubContext;
    private readonly LevelTagDbContext _DbContext;
    private readonly IDtoConverter<Contribution, ContributionDto> _DtoConverterForUsers;
    private readonly IDtoConverter<Contribution, ContributionDto> _DtoConverterForModerators;

    public ContributionBroadcaster(
        IHubContext<ContributionHub, IContributionHubClient> hubContext,
        LevelTagDbContext dbContext,
        IDtoConverterFactory<Contribution, ContributionDto, ContributionConversionParameters> dtoConverterFactory)
    {
        _HubContext = hubContext;
        _DbContext = dbContext;
        _DtoConverterForUsers = dtoConverterFactory.GetDtoConverter(GetConversionParameters(false));
        _DtoConverterForModerators = dtoConverterFactory.GetDtoConverter(GetConversionParameters(true));
    }

    public async Task BroadcastNewContributionAsync(Contribution contribution)
    {
        IQueryable<Contribution> contributionQuery = _DbContext.Set<Contribution>()
            .Where(dbContribution => dbContribution.Id == contribution.Id)
            .IncludeForDto(_DtoConverterForUsers)
            .IncludeForDto(_DtoConverterForModerators);

        Contribution reloadedContribution = await contributionQuery.FirstAsync();

        ContributionDto dtoForUsers = _DtoConverterForUsers.ToDto(reloadedContribution);
        ContributionDto dtoForModerators = _DtoConverterForModerators.ToDto(reloadedContribution);

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

        List<ContributionDto> dtosForUsers = await contributionQuery
            .SelectDto(_DtoConverterForUsers)
            .ToListAsync();

        List<ContributionDto> dtosForModerators = await contributionQuery
            .SelectDto(_DtoConverterForModerators)
            .ToListAsync();

        List<Task> broadcastTasks = [];

        broadcastTasks.AddRange(dtosForUsers.Select(contributionDto =>
            _HubContext.Clients.Group(ContributionHub.UsersGroupName).NewContribution(contributionDto)));
        broadcastTasks.AddRange(dtosForModerators.Select(contributionDto =>
            _HubContext.Clients.Group(ContributionHub.ModeratorsGroupName).NewContribution(contributionDto)));

        await Task.WhenAll(broadcastTasks);
    }

    private ContributionConversionParameters GetConversionParameters(bool ignoreAnonymity)
    {
        return new ContributionConversionParameters
        {
            IgnoreAnonymity = ignoreAnonymity,
            IncludeLevel = true,
            IncludeContributor = true,
        };
    }
}