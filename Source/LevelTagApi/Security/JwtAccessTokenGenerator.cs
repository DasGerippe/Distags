using DataClasses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LevelTagApi.Security;

internal class JwtAccessTokenGenerator : IAccessTokenGenerator
{
    private readonly AccessTokenSettings _AccessTokenSettings;
    private readonly RoleProvider _RoleProvider;

    public JwtAccessTokenGenerator(AccessTokenSettings accessTokenSettings, RoleProvider roleProvider)
    {
        _AccessTokenSettings = accessTokenSettings;
        _RoleProvider = roleProvider;
    }

    public SecurityToken GenerateAccessToken(User user)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_AccessTokenSettings.SecretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _AccessTokenSettings.Issuer,
            audience: _AccessTokenSettings.Audience,
            claims: GetUserClaims(user),
            expires: DateTime.UtcNow + _AccessTokenSettings.ExpirationDelay,
            signingCredentials: credentials);

        return token;
    }

    private IEnumerable<Claim> GetUserClaims(User user)
    {
        IEnumerable<Claim> userRoleClaims = _RoleProvider.GetAvailableRoles()
            .Join(
                user.Roles,
                availableRole => availableRole,
                userRole => userRole,
                (availableRole, userRole) => userRole)
            .Select(userRole => new Claim(ClaimTypes.Role, userRole));

        IEnumerable<Claim> userClaims = userRoleClaims.Append(new Claim("SteamId", user.Id.ToString()));

        return userClaims;
    }
}