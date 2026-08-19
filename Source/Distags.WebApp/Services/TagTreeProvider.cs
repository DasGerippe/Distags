using Distags.Contracts;
using Distags.WebApp.Models;

namespace Distags.WebApp.Services;

public class TagTreeProvider
{
    private readonly DistagsApiClient _DistagsApiClient;

    private IReadOnlyCollection<DisplayGroupWithTags>? _TagGroups;

    public TagTreeProvider(DistagsApiClient distagsApiClient)
    {
        _DistagsApiClient = distagsApiClient;
    }

    public async Task<IReadOnlyCollection<DisplayGroupWithTags>> GetTagTreeAsync()
    {
        _TagGroups ??= await BuildTagTreeAsync();
        return _TagGroups;
    }

    private async Task<List<DisplayGroupWithTags>> BuildTagTreeAsync()
    {
        List<DisplayGroupDto> displayGroups = await _DistagsApiClient.GetDisplayGroupsAsync();
        List<TagDto> tags = await _DistagsApiClient.GetTagsAsync();

        List<DisplayGroupWithTags> tagTree = displayGroups
            .Select(displayGroup => new DisplayGroupWithTags(
                displayGroup,
                tags.Where(tag => tag.DisplayGroupName == displayGroup.Name).ToList()))
            .ToList();

        return tagTree;
    }
}
