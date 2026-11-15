using Distags.Api.Contracts.DisplayGroups;
using Distags.Api.Contracts.Tags;
using Distags.WebApp.Models;

namespace Distags.WebApp.Services;

public class TagTreeProvider
{
    private readonly DisplayGroupProvider _DisplayGroupProvider;
    private readonly TagProvider _TagProvider;

    private readonly Lazy<Task> _InitializationTask;
    private IReadOnlyCollection<DisplayGroupWithTags>? _TagTree;

    public TagTreeProvider(DisplayGroupProvider displayGroupProvider, TagProvider tagProvider)
    {
        _DisplayGroupProvider = displayGroupProvider;
        _TagProvider = tagProvider;

        _InitializationTask = new Lazy<Task>(InitializeAsync, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private async Task InitializeAsync()
    {
        Task<List<DisplayGroupResponse>> displayGroupsTask = _DisplayGroupProvider.GetResourceAsync();
        Task<List<TagResponse>> tagsTask = _TagProvider.GetResourceAsync();

        List<DisplayGroupResponse> displayGroups = await displayGroupsTask.ConfigureAwait(false);
        List<TagResponse> tags = await tagsTask.ConfigureAwait(false);

        _TagTree = displayGroups
            .Select(displayGroup => new DisplayGroupWithTags(
                displayGroup,
                tags.Where(tag => tag.DisplayGroupName == displayGroup.Name).ToList()))
            .ToList();
    }

    public async Task<IReadOnlyCollection<DisplayGroupWithTags>> GetTagTreeAsync()
    {
        await _InitializationTask.Value;
        return _TagTree!;
    }
}
