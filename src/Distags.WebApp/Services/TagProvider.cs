using Distags.Api.Contracts.Tags;
using Distags.WebApp.DistagsApi;

namespace Distags.WebApp.Services;

public class TagProvider : ApiResourceProvider<List<TagResponse>>
{
    public TagProvider(DistagsApiClient distagsApiClient) : base(() => distagsApiClient.GetTagsAsync(), [])
    {
    }
}
