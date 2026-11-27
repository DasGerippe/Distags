namespace AutoTagging.Length;

public class LevelStats
{
    public long LevelId { get; init; }

    public string LevelName { get; init; } = string.Empty;

    public int EntryCount { get; init; }

    public TimeSpan WorldRecordTime { get; init; }

    public TimeSpan MedianTime { get; init; }

    public TimeSpan CiLowTime { get; init; }

    public TimeSpan CiHighTime { get; init; }
}
