using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LevelTagApi.Security;

public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly LevelTagDbContext _DbContext;

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        LevelTagDbContext dbContext)
        : base(options, logger, encoder)
    {
        _DbContext = dbContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(LevelTagApiConstants.SessionTokenHeader, out StringValues sessionToken))
            return AuthenticateResult.NoResult();

        UserSession? userSession = await _DbContext.Set<UserSession>()
            .Include(session => session.User)
            .Where(session => session.Token == sessionToken)
            .FirstOrDefaultAsync();

        if (userSession == null)
            return AuthenticateResult.Fail("Invalid session token.");

        User user = userSession.User;
        if (user.SteamId == null)
            return AuthenticateResult.Fail("Session belongs to unknown Steam user.");

        List<Claim> claims =
        [
            new Claim(Constants.Claims.SteamId, user.SteamId.Value.ToString()),
            .. user.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
        ];

        ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        AuthenticationTicket authTicket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(authTicket);
    }
}