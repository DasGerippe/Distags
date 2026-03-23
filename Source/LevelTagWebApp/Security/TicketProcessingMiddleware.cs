using LevelTagApi.Dtos;
using LevelTagWebApp.Services;
using Microsoft.Extensions.Primitives;

namespace LevelTagWebApp.Security;

public class TicketProcessingMiddleware
{
    private const string TicketParameterName = "ticket";

    private readonly RequestDelegate _Next;
    private readonly ILogger<TicketProcessingMiddleware> _Logger;

    public TicketProcessingMiddleware(RequestDelegate next, ILogger<TicketProcessingMiddleware> logger)
    {
        _Next = next;
        _Logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.Request.Query.TryGetValue(TicketParameterName, out StringValues value))
        {
            await _Next(httpContext);
            return;
        }

        try
        {
            string ticket = value.ToString();

            LevelTagApiClient levelTagApi = httpContext.RequestServices.GetRequiredService<LevelTagApiClient>();
            SessionResponse sessionResponse = await levelTagApi.PostRedeemTicketAsync(ticket);

            httpContext.Response.Cookies.Append(Constants.SessionTokenCookieName, sessionResponse.Token, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
            });
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "An error occurred while trying to process ticket parameter.");
        }

        IEnumerable<KeyValuePair<string, StringValues>> newQueryParams = httpContext.Request.Query
            .Where(queryParam => queryParam.Key != TicketParameterName);
        string redirectUrl = httpContext.Request.Path + QueryString.Create(newQueryParams).ToUriComponent();
        httpContext.Response.Redirect(redirectUrl);
    }
}