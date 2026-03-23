using DataClasses;
using Microsoft.IdentityModel.Tokens;

namespace LevelTagApi.Security;

public interface IAccessTokenGenerator
{
    SecurityToken GenerateAccessToken(User user);
}