namespace Distags.Contracts;

public class StatsSummaryDto
{
    public int LevelTagCount { get; set; }

    public int LevelCount { get; set; }

    public int UserContributionCount { get; set; }

    public int ContributingUserCount { get; set; }

    public int AutoTaggerContributionCount { get; set; }

    public int ContributingAutoTaggerCount { get; set; }
}
