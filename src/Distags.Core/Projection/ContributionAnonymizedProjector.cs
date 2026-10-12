using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ContributionAnonymizedProjector
{
    public static partial IQueryable<ContributionDto> SelectAnonymizedDto(this IQueryable<Contribution> contributions);

    [MapDerivedType<OfficialLevel, OfficialLevelDto>]
    [MapDerivedType<WorkshopLevel, WorkshopLevelDto>]
    private static partial LevelDto ProjectLevel(Level level);

    private static string ProjectTag(Tag tag) => tag.Name;

    [MapPropertyFromSource(nameof(ContributionUserDto.SteamId), Use = nameof(AnonymizeSteamId))]
    [MapPropertyFromSource(nameof(ContributionUserDto.SteamName), Use = nameof(AnonymizeSteamName))]
    [MapPropertyFromSource(nameof(ContributionUserDto.SteamAvatarHash), Use = nameof(AnonymizeSteamAvatarHash))]
    private static partial ContributionUserDto ProjectUser(User user);

    private static long? AnonymizeSteamId(User user) => user.IsAnonymous ? null : user.SteamId;

    private static string? AnonymizeSteamName(User user) => user.IsAnonymous ? null : user.SteamName;

    private static string? AnonymizeSteamAvatarHash(User user) => user.IsAnonymous ? null : user.SteamAvatarHash;
}
