using Distags.Api.Contracts.DisplayGroups;
using Distags.Api.Contracts.Tags;
using Distags.WebApp.DistagsApi;
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
        Task<ApiResult<List<DisplayGroupResponse>>> displayGroupsTask = _DistagsApiClient.GetDisplayGroupsAsync();
        Task<ApiResult<List<TagResponse>>> tagsTask = _DistagsApiClient.GetTagsAsync();

        ApiResult<List<DisplayGroupResponse>> displayGroupsResult = await displayGroupsTask.ConfigureAwait(false);
        ApiResult<List<TagResponse>> tagsResult = await tagsTask.ConfigureAwait(false);

        displayGroupsResult.EnsureSuccess();
        tagsResult.EnsureSuccess();

        List<DisplayGroupWithTags> tagTree = displayGroupsResult.Value
            .Select(displayGroup => new DisplayGroupWithTags(
                displayGroup,
                tagsResult.Value.Where(tag => tag.DisplayGroupName == displayGroup.Name).ToList()))
            .ToList();

        return tagTree;
    }
}
