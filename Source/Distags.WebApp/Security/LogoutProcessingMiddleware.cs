using Distags.WebApp.Services;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Distags.WebApp.Security;

public class LogoutProcessingMiddleware
{
    private readonly RequestDelegate _Next;
    private readonly ILogger<LogoutProcessingMiddleware> _Logger;

    public LogoutProcessingMiddleware(RequestDelegate next, ILogger<LogoutProcessingMiddleware> logger)
    {
        _Next = next;
        _Logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, LevelTagApiClient levelTagApi)
    {
        if (!httpContext.Request.Query.TryGetValue(Constants.LogoutQueryParameterName, out StringValues logoutValue))
        {
            await _Next(httpContext);
            return;
        }

        try
        {
            await PerformLogout(levelTagApi, logoutValue.ToString());
            DeleteSessionTokenCookie(httpContext);
        }
        catch (HttpRequestException httpRequestEx) when (httpRequestEx.StatusCode == HttpStatusCode.Unauthorized)
        {
            _Logger.LogDebug("Unauthorized to perform logout. SessionToken is probably already invalid.");
            DeleteSessionTokenCookie(httpContext);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while trying to log out.");
        }

        IEnumerable<KeyValuePair<string, StringValues>> newQueryParams = httpContext.Request.Query
            .Where(queryParam => queryParam.Key != Constants.LogoutQueryParameterName);
        string redirectUrl = httpContext.Request.Path + QueryString.Create(newQueryParams).ToUriComponent();
        httpContext.Response.Redirect(redirectUrl);
    }

    private Task PerformLogout(LevelTagApiClient levelTagApi, string logoutValue)
    {
        return logoutValue switch
        {
            Constants.LogoutAllQueryParameterValue => levelTagApi.PostLogoutAll(),
            _ => levelTagApi.PostLogout(),
        };
    }

    private void DeleteSessionTokenCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(Constants.SessionTokenCookieName);
    }
}