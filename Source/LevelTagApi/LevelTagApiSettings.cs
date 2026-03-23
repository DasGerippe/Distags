using System.ComponentModel.DataAnnotations;

namespace LevelTagApi;

public class LevelTagApiSettings
{
    [Required]
    public required string[] DefaultUserRoles { get; init; }

    [Required]
    public required TimeSpan TicketExpirationDelay { get; init; }

    [Required]
    public required TimeSpan SessionExpirationDelay { get; init; }
}