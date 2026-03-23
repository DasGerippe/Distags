using System.Buffers.Text;
using System.Security.Cryptography;

namespace LevelTagApi.Security;

public class RandomBase64UrlTokenGenerator : ITokenGenerator
{
    public string GenerateToken(int entropyByteCount)
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(entropyByteCount));
    }
}