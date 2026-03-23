namespace LevelTagApi.Dtos;

public class RevertContributionsResponse
{
    public int ContributionsReverted { get; set; }

    public int CollateralContributionsReverted { get; set; }

    public int LevelTagsAdded { get; set; }

    public int LevelTagsRemoved { get; set; }
}