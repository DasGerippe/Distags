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

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly LevelTagDbContext _DbContext;

    public ApiKeyAuthenticationHandler(
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
        if (authHeader?.Scheme != LevelTagApiConstants.AuthorizationHeaderSchemes.ApiKey)
            return AuthenticateResult.NoResult();

        string? apiKey = authHeader.Parameter;
        if (apiKey == null)
            return AuthenticateResult.Fail("Invalid API key.");

        App? app = await _DbContext.Set<App>()
            .FirstOrDefaultAsync(app => app.ApiKey == apiKey);

        if (app == null)
            return AuthenticateResult.Fail("Invalid API key.");

        List<Claim> claims =
        [
            new Claim(Constants.Claims.AppId, app.Id.ToString()),
            new Claim(ClaimTypes.Role, Constants.Roles.AutoTagger),
        ];
        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket authTicket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(authTicket);
    }
}