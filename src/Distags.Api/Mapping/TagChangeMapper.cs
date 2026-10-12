using Distags.Api.Contracts.Tags;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
public static partial class TagChangeMapper
{
    public static partial List<TagChangeResponse> Map(IReadOnlyCollection<TagChangeDto> tagChanges);

    public static partial TagChangeResponse Map(TagChangeDto tagChange);
}
