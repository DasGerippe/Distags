using Distags.Api.Contracts.Contributions;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
[UseStaticMapper(typeof(LevelMapper))]
public static partial class ContributionMapper
{
    public static partial ContributionResponse Map(ContributionDto contribution);
}
