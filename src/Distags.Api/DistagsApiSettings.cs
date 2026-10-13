using System.ComponentModel.DataAnnotations;

namespace Distags.Api;

public class DistagsApiSettings
{
    [Range(typeof(TimeSpan), "00:00:10", "01:00:00")]
    public TimeSpan TicketExpirationDelay { get; init; } = TimeSpan.FromMinutes(1);

    [Range(typeof(TimeSpan), "01:00:00", "90.00:00:00")]
    public TimeSpan SessionExpirationDelay { get; init; } = TimeSpan.FromDays(30);
}
