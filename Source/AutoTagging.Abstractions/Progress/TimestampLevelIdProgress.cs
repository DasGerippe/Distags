namespace AutoTagging.Abstractions.Progress;

public class TimestampLevelIdProgress : IAutoTaggerProgress
{
    public DateTimeOffset Timestamp { get; set; }

    public long LevelId { get; set; }
}
