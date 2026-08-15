namespace Distags.Core.Queries.LevelTags;

public class LevelTagsQuery : IQuery<List<string>>
{
    public long LevelId { get; set; }
}
