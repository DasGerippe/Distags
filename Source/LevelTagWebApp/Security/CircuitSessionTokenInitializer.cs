using Microsoft.AspNetCore.Components.Server.Circuits;

namespace LevelTagWebApp.Security;

public class CircuitSessionTokenInitializer : CircuitHandler
{
    private readonly IHttpContextAccessor _HttpContextAccessor;
    private readonly ISessionTokenProvider _SessionTokenProvider;

    public CircuitSessionTokenInitializer(
        IHttpContextAccessor httpContextAccessor,
        ISessionTokenProvider sessionTokenProvider)
    {
        _HttpContextAccessor = httpContextAccessor;
        _SessionTokenProvider = sessionTokenProvider;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = _HttpContextAccessor.HttpContext;
        if (httpContext?.Request.Cookies.TryGetValue(Constants.SessionTokenCookieName, out string? sessionToken) == true)
            _SessionTokenProvider.SessionToken = sessionToken;

        return Task.CompletedTask;
    }
}