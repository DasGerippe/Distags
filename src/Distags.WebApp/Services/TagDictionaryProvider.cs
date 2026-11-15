using Distags.Api.Contracts.Tags;

namespace Distags.WebApp.Services;

public class TagDictionaryProvider
{
    private readonly TagProvider _TagProvider;

    private readonly Lazy<Task> _InitializationTask;

    private Dictionary<string, TagResponse>? _TagDictionary;

    public TagDictionaryProvider(TagProvider tagProvider)
    {
        _TagProvider = tagProvider;
        _InitializationTask = new Lazy<Task>(InitializeAsync, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private async Task InitializeAsync()
    {
        List<TagResponse> tags = await _TagProvider.GetResourceAsync();
        _TagDictionary = tags.ToDictionary(tag => tag.Name);
    }

    public async Task<IReadOnlyDictionary<string, TagResponse>> GetTagDictionaryAsync()
    {
        await _InitializationTask.Value;
        return _TagDictionary!;
    }
}
