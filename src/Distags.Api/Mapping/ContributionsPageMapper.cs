using Distags.Api.Contracts.Contributions;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
[UseStaticMapper(typeof(ContributionMapper))]
public static partial class ContributionsPageMapper
{
    public static partial ContributionsPageResponse Map(ContributionsPage contributionsPage);
}
