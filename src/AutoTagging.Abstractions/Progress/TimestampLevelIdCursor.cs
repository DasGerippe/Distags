namespace AutoTagging.Abstractions.Progress;

public class TimestampLevelIdCursor
{
    public DateTimeOffset Timestamp { get; set; }

    public long LevelId { get; set; }
}
