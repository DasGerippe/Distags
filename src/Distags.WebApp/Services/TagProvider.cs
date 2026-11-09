using Distags.Api.Contracts.Tags;
using Distags.WebApp.DistagsApi;

namespace Distags.WebApp.Services;

public class TagProvider
{
    private readonly DistagsApiClient _DistagsApiClient;

    private bool _IsInitialized;
    private Dictionary<string, TagResponse> _Tags = [];

    public TagProvider(DistagsApiClient distagsApiClient)
    {
        _DistagsApiClient = distagsApiClient;
    }

    public async Task<IReadOnlyDictionary<string, TagResponse>> GetTagsDictionary()
    {
        await Initialize();

        return _Tags;
    }

    private async Task Initialize()
    {
        if (_IsInitialized)
            return;

        ApiResult<List<TagResponse>> tagsResult = await _DistagsApiClient.GetTagsAsync();
        if (tagsResult.IsSuccess)
            _Tags = tagsResult.Value.ToDictionary(tag => tag.Name);

        _IsInitialized = true;
    }
}
