using System.ComponentModel.DataAnnotations;

namespace LevelTagApi.Security;

public class AccessTokenSettings
{
    [Required, MinLength(32)]
    public required string SecretKey { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Required]
    public required TimeSpan ExpirationDelay { get; init; }
}