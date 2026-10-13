using Distags.Api.Contracts;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.UpdateUserSessionLastSeen;
using Distags.Core.Queries.UserBySessionToken;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Distags.Api.Security;

public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IRequestDispatcher _RequestDispatcher;

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IRequestDispatcher requestDispatcher)
        : base(options, logger, encoder)
    {
        _RequestDispatcher = requestDispatcher;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticationHeaderValue.TryParse(Request.Headers.Authorization.ToString(), out AuthenticationHeaderValue? authHeader);
        if (authHeader?.Scheme != DistagsApiConstants.AuthorizationHeaderSchemes.SessionToken)
            return AuthenticateResult.NoResult();

        string? sessionToken = authHeader.Parameter;
        if (string.IsNullOrEmpty(sessionToken))
            return AuthenticateResult.Fail("Invalid session token.");

        UserBySessionTokenQuery query = new UserBySessionTokenQuery { SessionToken = sessionToken };
        UserDto? user = await _RequestDispatcher.DispatchAsync(query);

        if (user == null)
            return AuthenticateResult.Fail("Invalid session token.");

        UpdateUserSessionLastSeenCommand command = new UpdateUserSessionLastSeenCommand { SessionToken = sessionToken };
        await _RequestDispatcher.DispatchAsync(command);

        bool userCanContribute = user.AcceptsGuidelines && !user.IsBanned;
        if (userCanContribute)
            user.Roles.Add(Constants.Roles.Contributor);

        List<Claim> claims =
        [
            new Claim(Constants.Claims.SteamId, user.SteamId.ToString()),
            new Claim(Constants.Claims.SessionToken, sessionToken),
            .. user.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
        ];

        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket authTicket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(authTicket);
    }
}
