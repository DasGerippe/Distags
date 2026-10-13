using Distags.Api.Contracts.Contributions;

namespace Distags.Api.Hubs;

public interface IContributionHubClient
{
    Task NewContribution(ContributionResponse contribution);
}
