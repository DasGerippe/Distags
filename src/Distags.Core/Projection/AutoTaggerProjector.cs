using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class AutoTaggerProjector
{
    public static partial IQueryable<AutoTaggerDto> SelectDto(this IQueryable<AutoTagger> autoTaggers);
}
