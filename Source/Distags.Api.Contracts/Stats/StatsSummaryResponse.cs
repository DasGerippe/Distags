namespace Distags.Api.Contracts.Stats;

public class StatsSummaryResponse
{
    public int LevelTagCount { get; init; }

    public int LevelCount { get; init; }

    public int UserContributionCount { get; init; }

    public int ContributingUserCount { get; init; }

    public int AutoTaggerContributionCount { get; init; }

    public int ContributingAutoTaggerCount { get; init; }
}
