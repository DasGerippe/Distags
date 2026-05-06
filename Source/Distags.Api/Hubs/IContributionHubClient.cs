using Distags.Contracts;

namespace Distags.Api.Hubs;

public interface IContributionHubClient
{
    Task NewContribution(ContributionDto contribution);
}