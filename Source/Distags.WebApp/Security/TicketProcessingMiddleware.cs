using Distags.Api.Contracts;
using Distags.Api.Contracts.Authentication;
using Distags.WebApp.Services;
using Microsoft.Extensions.Primitives;

namespace Distags.WebApp.Security;

public class TicketProcessingMiddleware
{
    private readonly RequestDelegate _Next;
    private readonly ILogger<TicketProcessingMiddleware> _Logger;

    public TicketProcessingMiddleware(RequestDelegate next, ILogger<TicketProcessingMiddleware> logger)
    {
        _Next = next;
        _Logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, DistagsApiClient levelTagApi)
    {
        if (!httpContext.Request.Query.TryGetValue(DistagsApiConstants.TicketQueryParameterName, out StringValues ticketValue))
        {
            await _Next(httpContext);
            return;
        }

        try
        {
            string ticket = ticketValue.ToString();
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
            .Where(queryParam => queryParam.Key != DistagsApiConstants.TicketQueryParameterName);
        string redirectUrl = httpContext.Request.Path + QueryString.Create(newQueryParams).ToUriComponent();
        httpContext.Response.Redirect(redirectUrl);
    }
}