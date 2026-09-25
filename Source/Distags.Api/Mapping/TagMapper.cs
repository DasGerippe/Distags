using Distags.Api.Contracts.Tags;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
public static partial class TagMapper
{
    public static partial TagResponse Map(TagDto tag);
}
