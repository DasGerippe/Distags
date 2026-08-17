using AspNet.Security.OpenId.Steam;
using Distags.Api.Contracts;
using Distags.Api.Contracts.Authentication;
using Distags.Api.Security;
using Distags.Core;
using Distags.Core.Commands.CreateOrUpdateUser;
using Distags.Core.Commands.CreateUserSession;
using Distags.Core.Commands.DeleteUserSession;
using Distags.Core.Commands.DeleteUserSessions;
using Distags.Core.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.SteamUser;
using System.Security.Claims;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ITicketStore<long> _TicketStore;
    private readonly ITokenGenerator _TokenGenerator;
    private readonly ISteamUser _SteamUser;
    private readonly ILogger<AuthenticationController> _Logger;

    private readonly string _SteamWebApiKey;

    public AuthenticationController(
        ITicketStore<long> ticketStore,
        ITokenGenerator tokenGenerator,
        ISteamUser steamUser,
        SteamWebApiSettings steamWebApiSettings,
        ILogger<AuthenticationController> logger)
    {
        _TicketStore = ticketStore;
        _TokenGenerator = tokenGenerator;
        _SteamUser = steamUser;
        _Logger = logger;

        _SteamWebApiKey = steamWebApiSettings.ApiKey;
    }

    [HttpGet("steam-login")]
    [AllowAnonymous]
    public IActionResult SteamLogin([FromQuery] string? returnUrl)
    {
        AuthenticationProperties authProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(SteamCallback), new { returnUrl }),
        };

        return Challenge(authProperties, SteamAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("steam-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> SteamCallback([FromQuery] string? returnUrl,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        AuthenticateResult result = await HttpContext.AuthenticateAsync(SteamAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded || result.Principal == null)
            return Unauthorized();

        string? claimedSteamId = result.Principal
            .FindFirstValue(ClaimTypes.NameIdentifier)?
            .Substring("https://steamcommunity.com/openid/id/".Length);

        if (!long.TryParse(claimedSteamId, out long steamId))
            return Unauthorized();

        if (!string.IsNullOrEmpty(returnUrl))
        {
            string ticket = _TokenGenerator.GenerateToken(8);
            _TicketStore.Store(ticket, steamId);

            string returnUrlWithTicket = QueryHelpers.AddQueryString(returnUrl, DistagsApiConstants.TicketQueryParameterName, ticket);
            return Redirect(returnUrlWithTicket);
        }

        string sessionToken = await CreateSessionForUserAsync(steamId, requestDispatcher);
        SessionResponse sessionResponse = new SessionResponse { Token = sessionToken };
        return Ok(sessionResponse);
    }

    [HttpPost("redeem-ticket")]
    [AllowAnonymous]
    public async Task<ActionResult<SessionResponse>> RedeemTicket(string ticket,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        long? steamId = _TicketStore.Retrieve(ticket);
        if (steamId == null)
            return BadRequest("Ticket is invalid or expired.");

        string sessionToken = await CreateSessionForUserAsync(steamId.Value, requestDispatcher);
        SessionResponse sessionResponse = new SessionResponse { Token = sessionToken };
        return Ok(sessionResponse);
    }

    [HttpGet("validate-session")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public IActionResult ValidateSession()
    {
        return NoContent();
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public async Task<IActionResult> Logout(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            string sessionToken = User.FindFirstValue(Constants.Claims.SessionToken)!;
            DeleteUserSessionCommand command = new DeleteUserSessionCommand { SessionToken = sessionToken };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch (UnknownUserSessionException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("logout-all")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public async Task<IActionResult> LogoutAll(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            long userSteamId = User.FindFirstValue<long>(Constants.Claims.SteamId)!.Value;
            DeleteUserSessionsCommand command = new DeleteUserSessionsCommand { UserSteamId = userSteamId };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch (UnknownUserException)
        {
            return Unauthorized();
        }
    }

    private async Task<string> CreateSessionForUserAsync(long steamId, IRequestDispatcher requestDispatcher)
    {
        PlayerSummary? playerSummary = await TryGetPlayerSummary(steamId);
        CreateOrUpdateUserCommand createOrUpdateUserCommand = new CreateOrUpdateUserCommand
        {
            SteamId = steamId,
            SteamName = playerSummary?.PersonaName,
            SteamAvatarHash = playerSummary?.AvatarHash,
        };
        await requestDispatcher.DispatchAsync(createOrUpdateUserCommand);

        string sessionToken = _TokenGenerator.GenerateToken(32);
        CreateUserSessionCommand createUserSessionCommand = new CreateUserSessionCommand
        {
            UserSteamId = steamId,
            SessionToken = sessionToken,
        };
        await requestDispatcher.DispatchAsync(createUserSessionCommand);

        return sessionToken;
    }

    private async Task<PlayerSummary?> TryGetPlayerSummary(long steamId)
    {
        try
        {
            GetPlayerSummariesParameters parameters = new GetPlayerSummariesParameters
            {
                Key = _SteamWebApiKey,
                SteamIds = [steamId],
            };

            await using IAsyncEnumerator<PlayerSummary> playerEnumerator =
                _SteamUser.GetPlayerSummariesAsync(parameters).GetAsyncEnumerator();

            await playerEnumerator.MoveNextAsync();
            return playerEnumerator.Current;
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "An error occurred while trying to get user data from Steam.");
            return null;
        }
    }
}