namespace LevelTagApi.Dtos;

public class TokensResponse
{
    public required string AccessToken { get; set; }

    public required DateTime AccessTokenExpiration { get; set; }

    public required string RefreshToken { get; set; }

    public required DateTime RefreshTokenExpiration { get; set; }
}