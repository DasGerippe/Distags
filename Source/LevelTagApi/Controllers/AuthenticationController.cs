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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private const string TicketQueryParamName = "ticket";

    private readonly LevelTagApiSettings _Settings;
    private readonly LevelTagDbContext _DbContext;
    private readonly ITicketStore<long> _TicketStore;
    private readonly ITicketGenerator _TicketGenerator;
    private readonly IAccessTokenGenerator _AccessTokenGenerator;
    private readonly IRefreshTokenGenerator _RefreshTokenGenerator;

    public AuthenticationController(
        LevelTagApiSettings settings,
        LevelTagDbContext dbContext,
        ITicketStore<long> ticketStore,
        ITicketGenerator ticketGenerator,
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _Settings = settings;
        _DbContext = dbContext;
        _TicketStore = ticketStore;
        _TicketGenerator = ticketGenerator;
        _AccessTokenGenerator = accessTokenGenerator;
        _RefreshTokenGenerator = refreshTokenGenerator;
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
            string ticket = _TicketGenerator.GenerateTicket();
            _TicketStore.Store(ticket, steamId);

            string returnUrlWithTicket = QueryHelpers.AddQueryString(returnUrl, TicketQueryParamName, ticket);
            return Redirect(returnUrlWithTicket);
        }

        TokensResponse tokensResponse = await CreateTokensResponseForNewUserSession(steamId);
        return Ok(tokensResponse);
    }

    [HttpPost("redeem-ticket")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensResponse>> RedeemTicket(string ticket)
    {
        long? steamId = _TicketStore.Retrieve(ticket);
        if (steamId == null)
            return BadRequest("Ticket is invalid or expired.");

        TokensResponse tokensResponse = await CreateTokensResponseForNewUserSession(steamId.Value);
        return Ok(tokensResponse);
    }

    [HttpPost("refresh-session")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensResponse>> RefreshAccessToken([FromQuery] string refreshToken)
    {
        UserSession? userSession = await GetUserSessionByRefreshToken(refreshToken);
        if (userSession == null)
            return BadRequest("Refresh token is invalid or expired.");

        RenewSession(userSession);
        await _DbContext.SaveChangesAsync();

        TokensResponse tokensResponse = CreateTokensResponse(userSession);
        return Ok(tokensResponse);
    }

    [HttpPost("steam-logout")]
    [AllowAnonymous]
    public async Task<IActionResult> SteamLogout([FromQuery] string refreshToken)
    {
        UserSession? userSession = await GetUserSessionByRefreshToken(refreshToken);
        if (userSession == null)
            return BadRequest("Refresh token is invalid or expired.");

        _DbContext.Remove(userSession);
        await _DbContext.SaveChangesAsync();

        return Ok();
    }

    private async Task<TokensResponse> CreateTokensResponseForNewUserSession(long steamId)
    {
        User user = GetOrCreateUser(steamId);
        UserSession userSession = CreateUserSession(user);
        await _DbContext.SaveChangesAsync();

        TokensResponse tokensResponse = CreateTokensResponse(userSession);
        return tokensResponse;
    }

    private User GetOrCreateUser(long steamId)
    {
        User? user = _DbContext.Set<User>().FirstOrDefault(user => user.Id == steamId);
        if (user == null)
        {
            user = new User
            {
                Id = steamId,
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
        };
        RenewSession(userSession);

        _DbContext.Add(userSession);
        return userSession;
    }

    private async Task<UserSession?> GetUserSessionByRefreshToken(string refreshToken)
    {
        return await _DbContext.Set<UserSession>()
            .Include(session => session.User)
            .Where(session => session.RefreshToken == refreshToken)
            .FirstOrDefaultAsync();
    }

    private void RenewSession(UserSession userSession)
    {
        userSession.RefreshToken = _RefreshTokenGenerator.GenerateRefreshToken();
        userSession.ExpiresOn = DateTime.UtcNow + _Settings.SessionExpirationDelay;
    }

    private TokensResponse CreateTokensResponse(UserSession userSession)
    {
        SecurityToken accessToken = _AccessTokenGenerator.GenerateAccessToken(userSession.User);

        JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();

        return new TokensResponse
        {
            AccessToken = jwtTokenHandler.WriteToken(accessToken),
            AccessTokenExpiration = accessToken.ValidTo,
            RefreshToken = userSession.RefreshToken,
            RefreshTokenExpiration = userSession.ExpiresOn,
        };
    }
}