using System.Buffers.Text;
using System.Security.Cryptography;

namespace Distags.Core.Security;

public class TokenGenerator
{
    public string GenerateToken(int entropyByteCount)
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(entropyByteCount));
    }
}
