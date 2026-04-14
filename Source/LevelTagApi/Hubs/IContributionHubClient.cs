using LevelTagApi.Dtos.Contribution;

namespace LevelTagApi.Hubs;

public interface IContributionHubClient
{
    Task NewContribution(ContributionDto contribution);
}