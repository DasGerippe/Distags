using Distags.Api.Contracts.DisplayGroups;
using Distags.WebApp.DistagsApi;

namespace Distags.WebApp.Services;

public class DisplayGroupProvider : ApiResourceProvider<List<DisplayGroupResponse>>
{
    public DisplayGroupProvider(DistagsApiClient distagsApiClient) : base(() => distagsApiClient.GetDisplayGroupsAsync(), [])
    {
    }
}
