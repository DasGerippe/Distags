using Distags.Api.Contracts.Stats;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class StatsSummaryMapper
{
    public static partial StatsSummaryResponse Map(StatsSummaryDto statsSummary);
}
