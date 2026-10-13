using Distags.Api.Contracts.Levels;
using Distags.Api.Contracts.Tags;

namespace Distags.Api.Contracts.Contributions;

public class ContributionResponse
{
    public required long Id { get; init; }

    public required LevelResponse Level { get; init; }

    public required List<TagChangeResponse> Changes { get; init; }

    public required ContributionUserResponse? User { get; init; }

    public required ContributionAppResponse? App { get; init; }

    public required DateTimeOffset ContributedOn { get; init; }
}
