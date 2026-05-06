using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.CreateOrUpdateUser;

public class CreateOrUpdateUserCommandHandler : ICommandHandler<CreateOrUpdateUserCommand>
{
    private readonly DistagsDbContext _DbContext;

    public CreateOrUpdateUserCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(CreateOrUpdateUserCommand command)
    {
        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == command.SteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (user == null)
            await CreateUserAsync(command.SteamId, command.SteamName, command.SteamAvatarHash).ConfigureAwait(false);
        else
            UpdateUser(user, command.SteamName, command.SteamAvatarHash);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task CreateUserAsync(long steamId, string? steamName, string? steamAvatarHash)
    {
        bool userBanExists = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == steamId)
            .AnyAsync()
            .ConfigureAwait(false);

        User user = new User
        {
            SteamId = steamId,
            SteamName = steamName,
            SteamAvatarHash = steamAvatarHash,
            IsBanned = userBanExists,
        };
        _DbContext.Add(user);
    }

    private void UpdateUser(User user, string? steamName, string? steamAvatarHash)
    {
        if (steamName != null)
            user.SteamName = steamName;

        if (steamAvatarHash != null)
            user.SteamAvatarHash = steamAvatarHash;
    }
}