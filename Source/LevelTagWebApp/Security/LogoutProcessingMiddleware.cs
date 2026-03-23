using LevelTagWebApp.Services;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace LevelTagWebApp.Security;

public class LogoutProcessingMiddleware
{
    private const string LogoutParameterName = "logout";

    private readonly RequestDelegate _Next;
    private readonly ILogger<LogoutProcessingMiddleware> _Logger;

    public LogoutProcessingMiddleware(RequestDelegate next, ILogger<LogoutProcessingMiddleware> logger)
    {
        _Next = next;
        _Logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.Request.Query.TryGetValue(LogoutParameterName, out StringValues value))
        {
            await _Next(httpContext);
            return;
        }

        try
        {
            LevelTagApiClient levelTagApi = httpContext.RequestServices.GetRequiredService<LevelTagApiClient>();
            await levelTagApi.PostLogout();
        }
        catch (HttpRequestException httpRequestEx) when (httpRequestEx.StatusCode == HttpStatusCode.Unauthorized)
        {
            _Logger.LogDebug("Unauthorized to perform logout. SessionToken is probably already invalid.");
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while trying to log out.");
        }

        httpContext.Response.Cookies.Delete(Constants.SessionTokenCookieName);

        IEnumerable<KeyValuePair<string, StringValues>> newQueryParams = httpContext.Request.Query
            .Where(queryParam => queryParam.Key != LogoutParameterName);
        string redirectUrl = httpContext.Request.Path + QueryString.Create(newQueryParams).ToUriComponent();
        httpContext.Response.Redirect(redirectUrl);
    }
}