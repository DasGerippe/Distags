namespace LevelTagApi.Security;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
}