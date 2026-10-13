namespace Distags.Core.Entities;

public class LevelTag
{
    public long LevelId { get; set; }

    public Level Level { get; set; }

    public long TagId { get; set; }

    public Tag Tag { get; set; }
}
