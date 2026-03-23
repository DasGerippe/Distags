using System.ComponentModel.DataAnnotations;

namespace SteamWebApi.DelegatingHandlers;

public class RetrySettings
{
    [Range(0, 10)]
    public int RetryCount { get; set; } = 3;

    [Range(typeof(TimeSpan), "00:00:00", "1.00:00:00")]
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    [Range(typeof(TimeSpan), "00:00:00", "1.00:00:00")]
    public TimeSpan MaxAcceptedRetryDelay { get; set; } = TimeSpan.FromHours(1);
}