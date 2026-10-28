using Distags.Api.Contracts.AutoTaggers;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class AutoTaggerMapper
{
    public static partial AutoTaggerResponse Map(AutoTaggerDto autoTagger);
}
