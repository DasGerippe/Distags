using Distags.Api.Contracts.LogicGroups;
using Distags.WebApp.DistagsApi;

namespace Distags.WebApp.Services;

public class LogicGroupProvider : ApiResourceProvider<List<LogicGroupResponse>>
{
    public LogicGroupProvider(DistagsApiClient distagsApiClient) : base(() => distagsApiClient.GetLogicGroupsAsync(), [])
    {
    }
}
