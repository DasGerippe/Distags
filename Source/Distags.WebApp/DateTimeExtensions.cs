namespace Distags.WebApp;

public static class DateTimeExtensions
{
    public static string ToElapsedString(this DateTimeOffset dateTime)
    {
        TimeSpan elapsed = DateTimeOffset.UtcNow - dateTime;

        if (elapsed.TotalSeconds < 60)
            return Resource.DateTimeSecondsAgo;

        if (elapsed.TotalMinutes < 60)
            return string.Format(Resource.DateTimeMinutesAgo, elapsed.Minutes);

        if (elapsed.TotalHours < 24)
            return string.Format(Resource.DateTimeHoursAgo, elapsed.TotalHours.ToString("F1"));

        if (elapsed.TotalDays < 30)
            return string.Format(Resource.DateTimeDaysAgo, elapsed.Days);

        return dateTime.ToString("d");
    }

    public static string ToApproximateString(this DateTimeOffset dateTime)
    {
        return dateTime.ToString(DateTimeOffset.UtcNow - dateTime < TimeSpan.FromDays(180) ? "m" : "y");
    }
}