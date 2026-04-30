using Distags.Core.Commands;
using Distags.Core.Commands.UpdateUserSessionLastSeen;
using Distags.Core.Dtos;
using Distags.Core.Queries;
using Distags.Core.Queries.UserBySessionToken;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LevelTagApi.Security;

public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IQueryHandler<UserBySessionTokenQuery, UserDto?> _UserBySessionTokenQueryHandler;
    private readonly ICommandHandler<UpdateUserSessionLastSeenCommand> _UpdateUserSessionLastSeenCommandHandler;

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IQueryHandler<UserBySessionTokenQuery, UserDto?> userBySessionTokenQueryHandler,
        ICommandHandler<UpdateUserSessionLastSeenCommand> updateUserSessionLastSeenCommandHandler)
        : base(options, logger, encoder)
    {
        _UserBySessionTokenQueryHandler = userBySessionTokenQueryHandler;
        _UpdateUserSessionLastSeenCommandHandler = updateUserSessionLastSeenCommandHandler;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticationHeaderValue.TryParse(Request.Headers.Authorization.ToString(), out AuthenticationHeaderValue? authHeader);
        if (authHeader?.Scheme != LevelTagApiConstants.AuthorizationHeaderSchemes.SessionToken)
            return AuthenticateResult.NoResult();

        string? sessionToken = authHeader.Parameter;
        if (string.IsNullOrEmpty(sessionToken))
            return AuthenticateResult.Fail("Invalid session token.");

        UserBySessionTokenQuery query = new UserBySessionTokenQuery { SessionToken = sessionToken };
        UserDto? user = await _UserBySessionTokenQueryHandler.HandleAsync(query);

        if (user == null)
            return AuthenticateResult.Fail("Invalid session token.");

        UpdateUserSessionLastSeenCommand command = new UpdateUserSessionLastSeenCommand { SessionToken = sessionToken };
        await _UpdateUserSessionLastSeenCommandHandler.HandleAsync(command);

        bool userCanTag = user.AcceptsGuidelines && !user.IsBanned;

        List<Claim> claims =
        [
            new Claim(Constants.Claims.SteamId, user.SteamId.ToString()),
            new Claim(Constants.Claims.SessionToken, sessionToken),
            new Claim(Constants.Claims.CanTag, userCanTag.ToString()),
            .. user.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
        ];

        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket authTicket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(authTicket);
    }
}