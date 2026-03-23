using System.ComponentModel.DataAnnotations;

namespace LevelTagApi;

public class LevelTagApiSettings
{
    public string[] DefaultUserRoles { get; init; } = [];

    [Range(typeof(TimeSpan), "00:00:10", "01:00:00")]
    public TimeSpan TicketExpirationDelay { get; init; } = TimeSpan.FromMinutes(1);

    [Range(typeof(TimeSpan), "01:00:00", "90.00:00:00")]
    public TimeSpan SessionExpirationDelay { get; init; } = TimeSpan.FromDays(30);
}