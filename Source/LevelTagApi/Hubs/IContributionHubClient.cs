using Distags.Contracts;

namespace LevelTagApi.Hubs;

public interface IContributionHubClient
{
    Task NewContribution(ContributionDto contribution);
}