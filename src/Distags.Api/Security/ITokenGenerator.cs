namespace Distags.Api.Security;

public interface ITokenGenerator
{
    string GenerateToken(int entropyByteCount);
}
