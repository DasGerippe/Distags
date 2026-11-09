using Distags.Api.Contracts.Tags;
using Distags.WebApp.DistagsApi;

namespace Distags.WebApp.Services;

public class TagProvider
{
    private readonly DistagsApiClient _DistagsApiClient;

    private readonly Lazy<Task> _InitializationTask;

    private Dictionary<string, TagResponse> _Tags = [];

    public TagProvider(DistagsApiClient distagsApiClient)
    {
        _DistagsApiClient = distagsApiClient;

        _InitializationTask = new Lazy<Task>(InitializeAsync, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private async Task InitializeAsync()
    {
        ApiResult<List<TagResponse>> tagsResult = await _DistagsApiClient.GetTagsAsync();
        _Tags = tagsResult.Value?.ToDictionary(tag => tag.Name) ?? [];
    }

    public async Task<IReadOnlyDictionary<string, TagResponse>> GetTagsDictionary()
    {
        await _InitializationTask.Value;

        return _Tags;
    }
}
