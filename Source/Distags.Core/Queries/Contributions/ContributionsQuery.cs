using Distags.Contracts;

namespace Distags.Core.Queries.Contributions;

public class ContributionsQuery : IQuery<ContributionsPage>
{
    public long? UserSteamId { get; set; }

    public string? AppName { get; set; }

    public long? LevelId { get; set; }

    public string? TagName { get; set; }

    public DateTimeOffset? ContributedAfter { get; set; }

    public DateTimeOffset? ContributedBefore { get; set; }

    public bool DescendingOrder { get; set; } = true;

    public bool IgnoreAnonymity { get; set; }

    public int Limit { get; set; }
}