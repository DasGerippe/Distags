using AspNet.Security.OpenId.Steam;
using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly LevelTagApiSettings _Settings;
    private readonly LevelTagDbContext _DbContext;
    private readonly ITicketStore<long> _TicketStore;
    private readonly ITokenGenerator _TokenGenerator;

    public AuthenticationController(
        LevelTagApiSettings settings,
        LevelTagDbContext dbContext,
        ITicketStore<long> ticketStore,
        ITokenGenerator tokenGenerator)
    {
        _Settings = settings;
        _DbContext = dbContext;
        _TicketStore = ticketStore;
        _TokenGenerator = tokenGenerator;
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
    public async Task<IActionResult> SteamCallback([FromQuery] string? returnUrl = null)
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

            string returnUrlWithTicket = QueryHelpers.AddQueryString(returnUrl, LevelTagApiConstants.TicketQueryParameterName, ticket);
            return Redirect(returnUrlWithTicket);
        }

        SessionResponse sessionResponse = await CreateSessionResponseForNewUserSession(steamId);
        return Ok(sessionResponse);
    }

    [HttpPost("redeem-ticket")]
    [AllowAnonymous]
    public async Task<ActionResult<SessionResponse>> RedeemTicket(string ticket)
    {
        long? steamId = _TicketStore.Retrieve(ticket);
        if (steamId == null)
            return BadRequest("Ticket is invalid or expired.");

        SessionResponse sessionResponse = await CreateSessionResponseForNewUserSession(steamId.Value);
        return Ok(sessionResponse);
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = Constants.Session.AuthenticationScheme)]
    public async Task<IActionResult> Logout([FromHeader(Name = LevelTagApiConstants.SessionTokenHeader)] string sessionToken)
    {
        UserSession? userSession = await _DbContext.Set<UserSession>()
            .Where(session => session.Token == sessionToken)
            .FirstOrDefaultAsync();

        if (userSession == null)
            return Unauthorized();

        _DbContext.Remove(userSession);
        await _DbContext.SaveChangesAsync();

        return Ok();
    }

    private async Task<SessionResponse> CreateSessionResponseForNewUserSession(long steamId)
    {
        User user = GetOrCreateUser(steamId);
        UserSession userSession = CreateUserSession(user);
        await _DbContext.SaveChangesAsync();

        SessionResponse sessionResponse = new SessionResponse { Token = userSession.Token };
        return sessionResponse;
    }

    private User GetOrCreateUser(long steamId)
    {
        User? user = _DbContext.Set<User>().FirstOrDefault(user => user.SteamId == steamId);
        if (user == null)
        {
            user = new User
            {
                SteamId = steamId,
                Roles = _Settings.DefaultUserRoles.ToList(),
            };
            _DbContext.Add(user);
        }

        return user;
    }

    private UserSession CreateUserSession(User user)
    {
        UserSession userSession = new UserSession
        {
            User = user,
            Token = _TokenGenerator.GenerateToken(32),
        };

        _DbContext.Add(userSession);
        return userSession;
    }
}