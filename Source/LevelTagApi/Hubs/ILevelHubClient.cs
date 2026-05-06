using Distags.Contracts;

namespace LevelTagApi.Hubs;

public interface ILevelHubClient
{
    Task TagsChanged(IReadOnlyCollection<TagChangeDto> tagChanges);
}