using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class TagMapper : IEntityMapper<Tag, TagDto>
{
    public Expression<Func<Tag, TagDto>> Expression => tag => new TagDto
    {
        Name = tag.Name,
        DisplayName = tag.DisplayName,
        Description = tag.Description,
        Position = tag.Position,
        Exclusivity = tag.Exclusivity,
        IsAutoTag = tag.IsAutoTag,
    };
}