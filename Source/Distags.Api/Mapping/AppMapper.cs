using Distags.Api.Contracts.Apps;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
public static partial class AppMapper
{
    public static partial AppResponse Map(AppDto app);
}
