using Distags.Contracts;
using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.CreateOrUpdateUser;

public class CreateOrUpdateUserCommandHandler : ICommandHandler<CreateOrUpdateUserCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly DistagsCoreSettings _Settings;

    public CreateOrUpdateUserCommandHandler(DistagsDbContext dbContext, DistagsCoreSettings settings)
    {
        _DbContext = dbContext;
        _Settings = settings;
    }

    public async Task HandleAsync(CreateOrUpdateUserCommand command)
    {
        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.SteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (user == null)
            user = await CreateUserAsync(command.SteamId, command.SteamName, command.SteamAvatarHash).ConfigureAwait(false);
        else
            UpdateUser(user, command.SteamName, command.SteamAvatarHash);

        if (_Settings.AdministratorSteamIds.Contains(command.SteamId) && !user.Roles.Contains(Constants.UserRoles.Administrator))
            user.Roles.Add(Constants.UserRoles.Administrator);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task<User> CreateUserAsync(long steamId, string? steamName, string? steamAvatarHash)
    {
        User user = new User
        {
            SteamId = steamId,
            SteamName = steamName,
            SteamAvatarHash = steamAvatarHash,
        };

        UserBan? userBan = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == steamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (userBan != null)
        {
            user.IsBanned = true;
            userBan.User = user;
        }

        _DbContext.Add(user);
        return user;
    }

    private void UpdateUser(User user, string? steamName, string? steamAvatarHash)
    {
        if (steamName != null)
            user.SteamName = steamName;

        if (steamAvatarHash != null)
            user.SteamAvatarHash = steamAvatarHash;
    }
}