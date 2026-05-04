using Distags.Core.Dtos;

namespace LevelTagApi.Hubs;

public interface IContributionHubClient
{
    Task NewContribution(ContributionDto contribution);
}