using Distags.Contracts;
using Distags.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Distags.Core.Mapping;

[Mapper]
public static partial class TagChangeMapper
{
    public static partial List<TagChangeDto> Map(IReadOnlyCollection<TagChange> tagChanges);

    public static partial TagChangeDto Map(TagChange tagChange);
}
