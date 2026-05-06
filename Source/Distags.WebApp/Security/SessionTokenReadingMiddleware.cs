namespace Distags.WebApp.Security;

public class SessionTokenReadingMiddleware
{
    private readonly RequestDelegate _Next;

    public SessionTokenReadingMiddleware(RequestDelegate next)
    {
        _Next = next;
    }

    public Task InvokeAsync(HttpContext httpContext, ISessionTokenProvider sessionTokenProvider)
    {
        if (httpContext.Request.Cookies.TryGetValue(Constants.SessionTokenCookieName, out string? sessionToken))
            sessionTokenProvider.SessionToken = sessionToken;

        return _Next(httpContext);
    }
}