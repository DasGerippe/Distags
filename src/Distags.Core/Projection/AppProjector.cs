using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Projection;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class AppProjector
{
    public static partial IQueryable<AppDto> SelectDto(this IQueryable<App> apps);
}
