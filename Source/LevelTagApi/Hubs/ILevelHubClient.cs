using Distags.Core.Dtos;

namespace LevelTagApi.Hubs;

public interface ILevelHubClient
{
    Task TagsChanged(IReadOnlyCollection<TagChangeDto> tagChanges);
}