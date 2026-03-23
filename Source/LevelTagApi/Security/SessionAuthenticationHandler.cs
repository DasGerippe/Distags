using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
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
        AuthenticationHeaderValue.TryParse(Request.Headers.Authorization.ToString(), out AuthenticationHeaderValue? authHeader);
        if (authHeader?.Scheme != LevelTagApiConstants.AuthorizationHeaderSchemes.SessionToken)
            return AuthenticateResult.NoResult();

        string? sessionToken = authHeader.Parameter;
        if (sessionToken == null)
            return AuthenticateResult.Fail("Invalid session token.");

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
            new Claim(Constants.Claims.SessionToken, userSession.Token),
            .. user.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
        ];

        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket authTicket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(authTicket);
    }
}