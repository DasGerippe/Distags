using Distags.Api.Contracts.Levels;
using Distags.Api.Contracts.Tags;

namespace Distags.Api.Contracts.Contributions;

public class ContributionResponse
{
    public required long Id { get; set; }

    public required LevelResponse Level { get; set; }

    public required List<TagChangeResponse> Changes { get; set; }

    public required ContributionUserResponse? User { get; set; }

    public required ContributionAppResponse? App { get; set; }

    public required DateTimeOffset ContributedOn { get; set; }
}
