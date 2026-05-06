using System.Buffers.Text;
using System.Security.Cryptography;

namespace Distags.Api.Security;

public class RandomBase64UrlTokenGenerator : ITokenGenerator
{
    public string GenerateToken(int entropyByteCount)
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(entropyByteCount));
    }
}