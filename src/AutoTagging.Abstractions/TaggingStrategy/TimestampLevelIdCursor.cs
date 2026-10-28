namespace AutoTagging.Abstractions.TaggingStrategy;

public class TimestampLevelIdCursor
{
    public DateTimeOffset Timestamp { get; set; }

    public long LevelId { get; set; }
}
