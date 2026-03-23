namespace LevelTagWebApp;

public static class DateTimeExtensions
{
    public static string ToElapsedString(this DateTime dateTime)
    {
        TimeSpan ago = DateTime.UtcNow - dateTime;

        if (ago.TotalSeconds < 60)
            return Resource.DateTimeSecondsAgo;

        if (ago.TotalMinutes < 60)
            return string.Format(Resource.DateTimeMinutesAgo, ago.Minutes);

        if (ago.TotalHours < 24)
            return string.Format(Resource.DateTimeHoursAgo, ago.TotalHours.ToString("F1"));

        if (ago.TotalDays < 30)
            return string.Format(Resource.DateTimeDaysAgo, ago.Days);

        return dateTime.ToString("d");
    }
}