using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(LevelMapper))]
public static partial class ContributionAnonymizedMapper
{
    public static partial ContributionDto Map(Contribution contribution);

    [MapPropertyFromSource(nameof(ContributionUserDto.SteamId), Use = nameof(AnonymizeSteamId))]
    [MapPropertyFromSource(nameof(ContributionUserDto.SteamName), Use = nameof(AnonymizeSteamName))]
    [MapPropertyFromSource(nameof(ContributionUserDto.SteamAvatarHash), Use = nameof(AnonymizeSteamAvatarHash))]
    private static partial ContributionUserDto MapUser(User user);

    private static long? AnonymizeSteamId(User user) => user.IsAnonymous ? null : user.SteamId;

    private static string? AnonymizeSteamName(User user) => user.IsAnonymous ? null : user.SteamName;

    private static string? AnonymizeSteamAvatarHash(User user) => user.IsAnonymous ? null : user.SteamAvatarHash;
}
