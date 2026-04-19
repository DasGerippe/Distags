using LevelTagApi.Dtos.Contributions;

namespace LevelTagApi.Hubs;

public interface IContributionHubClient
{
    Task NewContribution(ContributionDto contribution);
}