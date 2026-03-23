using System.Security.Cryptography;

namespace LevelTagApi.Security;

public class RandomRefreshTokenGenerator : IRefreshTokenGenerator
{
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}