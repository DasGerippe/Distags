using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TagProjector
{
    public static partial IQueryable<TagDto> SelectDto(this IQueryable<Tag> tags);
}
