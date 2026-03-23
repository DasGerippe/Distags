using AspNet.Security.OpenId.Steam;
using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Management;
using LevelTagApi.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SteamWebApi;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.SteamUser;
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
    private readonly IUserManager _UserManager;
    private readonly ISteamUser _SteamUser;
    private readonly ILogger<AuthenticationController> _Logger;

    private readonly string _SteamWebApiKey;

    public AuthenticationController(
        LevelTagApiSettings settings,
        LevelTagDbContext dbContext,
        ITicketStore<long> ticketStore,
        ITokenGenerator tokenGenerator,
        IUserManager userManager,
        ISteamUser steamUser,
        SteamWebApiSettings steamWebApiSettings,
        ILogger<AuthenticationController> logger)
    {
        _Settings = settings;
        _DbContext = dbContext;
        _TicketStore = ticketStore;
        _TokenGenerator = tokenGenerator;
        _UserManager = userManager;
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

    [HttpGet("validate-session")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public IActionResult ValidateSession()
    {
        return Ok();
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public async Task<IActionResult> Logout()
    {
        UserSession? userSession = await _UserManager.GetCurrentUserSessionAsync();
        if (userSession == null)
            return Unauthorized();

        _DbContext.Remove(userSession);
        await _DbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("logout-all")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public async Task<IActionResult> LogoutAll()
    {
        User? user = await _UserManager.GetCurrentUserAsync(includes: nameof(user.Sessions));
        if (user == null)
            return Unauthorized();

        _DbContext.RemoveRange(user.Sessions);
        await _DbContext.SaveChangesAsync();

        return Ok();
    }

    private async Task<SessionResponse> CreateSessionResponseForNewUserSession(long steamId)
    {
        User user = await GetOrCreateUser(steamId);
        UserSession userSession = CreateUserSession(user);
        await _DbContext.SaveChangesAsync();

        SessionResponse sessionResponse = new SessionResponse { Token = userSession.Token };
        return sessionResponse;
    }

    private async Task<User> GetOrCreateUser(long steamId)
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

        await TrySetSteamData(user);

        return user;
    }

    private async Task TrySetSteamData(User user)
    {
        try
        {
            GetPlayerSummariesParameters parameters = new GetPlayerSummariesParameters
            {
                Key = _SteamWebApiKey,
                SteamIds = [user.SteamId!.Value],
            };

            IAsyncEnumerator<PlayerSummary> playerEnumerator = _SteamUser.GetPlayerSummariesAsync(parameters).GetAsyncEnumerator();
            await playerEnumerator.MoveNextAsync();

            PlayerSummary playerSummary = playerEnumerator.Current;
            user.SteamName = playerSummary.PersonaName;
            user.SteamAvatarHash = playerSummary.AvatarHash;
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "An error occurred while trying to get user data from Steam.");
        }
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