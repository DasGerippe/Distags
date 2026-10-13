using Distags.Api.Contracts.Tags;

namespace Distags.Api.Hubs;

public interface ILevelHubClient
{
    Task TagsChanged(IReadOnlyCollection<TagChangeResponse> tagChanges);
}
