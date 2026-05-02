namespace Distags.Core.Queries.Contributions;

public class ContributionsQuery : IQuery<ContributionsPage>
{
    public long? UserSteamId { get; set; }

    public string? AppName { get; set; }

    public long? LevelId { get; set; }

    public string? TagName { get; set; }

    public bool IgnoreAnonymity { get; set; }

    public int Limit { get; set; }

    public string? Cursor { get; set; }
}