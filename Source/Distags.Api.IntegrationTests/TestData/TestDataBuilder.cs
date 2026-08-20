using Distags.Api.IntegrationTests.Infrastructure;
using Distags.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Distags.Api.IntegrationTests.TestData;

/// <summary>
/// Writes test data to the database with admin permissions - no API 4 u sir.
/// </summary>
public static class TestDataBuilder
{
    public static Task<Creator> AddCreatorAsync(
        this DatabaseFixture database,
        long id,
        string name = "Test Creator")
    {
        Creator creator = new Creator
        {
            Id = id,
            Name = name,
            AvatarHash = string.Empty,
            CachedOn = DateTimeOffset.UtcNow,
        };

        return database.AddAsync(creator);
    }

    public static Task<Level> AddLevelAsync(
        this DatabaseFixture database,
        long id,
        long creatorId,
        string name = "Test Level")
    {
        Level level = new Level
        {
            Id = id,
            Name = name,
            CreatorId = creatorId,
            FileName = $"{name}.bytes",
            FileUrl = $"https://das.invalid.gerippe/levels/{id}",
            ImageUrl = $"https://der.invalid.puma/levels/{id}.jpg",
            CreatedOn = DateTimeOffset.UtcNow,
            UpdatedOn = DateTimeOffset.UtcNow,
        };

        return database.AddAsync(level);
    }

    /// <summary>
    /// Creates a user that can authenticate.
    /// </summary>
    [Experimental("DISTAGS001", Message = "Not proven by any test yet.")] // TODO
    public static Task<User> AddUserAsync(
        this DatabaseFixture database,
        long steamId,
        params string[] roles)
    {
        User user = new User
        {
            SteamId = steamId,
            SteamName = $"Test User {steamId}",
            AcceptsGuidelines = true,
            IsBanned = false,
            Roles = roles.ToList(),
        };

        return database.AddAsync(user);
    }

    /// <summary>
    /// Creates a session token. Authentication does not check expiry, so any LastSeenOn works.
    /// </summary>
    [Experimental("DISTAGS001", Message = "Not proven by any test yet.")] // TODO
    public static Task<UserSession> AddUserSessionAsync(
        this DatabaseFixture database,
        long userId,
        string token)
    {
        UserSession userSession = new UserSession
        {
            Token = token,
            UserId = userId,
            LastSeenOn = DateTimeOffset.UtcNow,
        };

        return database.AddAsync(userSession);
    }

    /// <summary>
    /// Creates a bot app. Send its key as the header "Authorization: ApiKey {key}".
    /// </summary>
    [Experimental("DISTAGS001", Message = "Not proven by any test yet.")] // TODO
    public static Task<App> AddAppAsync(this DatabaseFixture database, string name, string apiKey)
    {
        App app = new App
        {
            Name = name,
            ApiKey = apiKey,
        };

        return database.AddAsync(app);
    }

    private static async Task<TEntity> AddAsync<TEntity>(this DatabaseFixture database, TEntity entity)
        where TEntity : class
    {
        await database.WithDbContextAsync(async dbContext =>
        {
            dbContext.Add(entity);
            await dbContext.SaveChangesAsync();
        });

        return entity;
    }
}
