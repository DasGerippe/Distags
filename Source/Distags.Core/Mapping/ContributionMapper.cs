using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(LevelMapper))]
public static partial class ContributionMapper
{
    public static partial ContributionDto Map(Contribution contribution);
}
