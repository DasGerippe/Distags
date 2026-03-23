namespace LevelTagApi.Security;

public interface ITokenGenerator
{
    string GenerateToken(int entropyByteCount);
}