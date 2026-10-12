using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UserBanProjector
{
    public static partial IQueryable<UserBanDto> SelectDto(this IQueryable<UserBan> userBans);

    [MapPropertyFromSource(nameof(UserBanDto.SteamName), Use = nameof(ProjectSteamName))]
    [MapPropertyFromSource(nameof(UserBanDto.SteamAvatarHash), Use = nameof(ProjectSteamAvatarHash))]
    private static partial UserBanDto ProjectUserBan(UserBan userBan);

    private static string? ProjectSteamName(UserBan userBan) => userBan.User!.SteamName;

    private static string? ProjectSteamAvatarHash(UserBan userBan) => userBan.User!.SteamAvatarHash;
}
