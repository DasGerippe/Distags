namespace AutoTagging.Length;

public class LengthTagResolver
{
    private static readonly LengthTagInterval[] LengthTagIntervals =
    [
        new LengthTagInterval("30-sec", TimeSpan.Zero, TimeSpan.FromSeconds(40)),
        new LengthTagInterval("1-min", TimeSpan.FromSeconds(40), TimeSpan.FromMinutes(1.5)),
        new LengthTagInterval("2-min", TimeSpan.FromMinutes(1.5), TimeSpan.FromMinutes(2.5)),
        new LengthTagInterval("3-min", TimeSpan.FromMinutes(2.5), TimeSpan.FromMinutes(4)),
        new LengthTagInterval("5-min", TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(6.5)),
        new LengthTagInterval("8-min", TimeSpan.FromMinutes(6.5), TimeSpan.FromMinutes(10)),
        new LengthTagInterval("15-min", TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(30)),
        new LengthTagInterval("45-min", TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(60)),
        new LengthTagInterval("hours", TimeSpan.FromMinutes(60), TimeSpan.MaxValue),
    ];

    public static LengthTagInterval GetLengthTagInterval(TimeSpan length)
    {
        return LengthTagIntervals.First(lengthTagInterval => lengthTagInterval.Contains(length));
    }

    public static string GetLengthTag(TimeSpan length)
    {
        return LengthTagIntervals.First(lengthTagInterval => lengthTagInterval.Contains(length)).TagName;
    }

    public static string GetWrLengthTag(TimeSpan length)
    {
        return $"wr-{GetLengthTag(length)}";
    }
}
