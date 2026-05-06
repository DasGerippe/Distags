using Distags.Contracts;

namespace Distags.Api.Hubs;

public interface ILevelHubClient
{
    Task TagsChanged(IReadOnlyCollection<TagChangeDto> tagChanges);
}