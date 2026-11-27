namespace AutoTagging.Length;

public class LengthTagInterval
{
    public string TagName { get; }

    public TimeSpan MinTime { get; }

    public TimeSpan MaxTime { get; }

    public TimeSpan Length => MaxTime - MinTime;

    public LengthTagInterval(string tagName, TimeSpan minTime, TimeSpan maxTime)
    {
        TagName = tagName;
        MinTime = minTime;
        MaxTime = maxTime;
    }

    public bool Contains(TimeSpan time)
    {
        return time >= MinTime && time < MaxTime;
    }
}
