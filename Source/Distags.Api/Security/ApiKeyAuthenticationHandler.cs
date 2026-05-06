using Distags.Api.Contracts;
using Distags.Contracts;
using Distags.Core.Queries;
using Distags.Core.Queries.AppByApiKey;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Distags.Api.Security;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IQueryHandler<AppByApiKeyQuery, AppDto?> _AppByApiKeyQueryHandler;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IQueryHandler<AppByApiKeyQuery, AppDto?> appByApiKeyQueryHandler)
        : base(options, logger, encoder)
    {
        _AppByApiKeyQueryHandler = appByApiKeyQueryHandler;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticationHeaderValue.TryParse(Request.Headers.Authorization.ToString(), out AuthenticationHeaderValue? authHeader);
        if (authHeader?.Scheme != LevelTagApiConstants.AuthorizationHeaderSchemes.ApiKey)
            return AuthenticateResult.NoResult();

        string? apiKey = authHeader.Parameter;
        if (apiKey == null)
            return AuthenticateResult.Fail("Invalid API key.");

        AppByApiKeyQuery query = new AppByApiKeyQuery { ApiKey = apiKey };
        AppDto? app = await _AppByApiKeyQueryHandler.HandleAsync(query);

        if (app == null)
            return AuthenticateResult.Fail("Invalid API key.");

        List<Claim> claims =
        [
            new Claim(Constants.Claims.AppName, app.Name),
            new Claim(ClaimTypes.Role, Constants.Roles.Tagger),
        ];
        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket authTicket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(authTicket);
    }
}