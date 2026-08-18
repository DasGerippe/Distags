using Distags.Contracts;
using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class TagMapper : IEntityMapper<Tag, TagDto>
{
    public Expression<Func<Tag, TagDto>> Expression => tag => new TagDto
    {
        Name = tag.Name,
        DisplayName = tag.DisplayName,
        Description = tag.Description,
        DisplayGroupName = tag.DisplayGroup.Name,
        Position = tag.Position,
        LogicGroupName = tag.LogicGroup!.Name,
        IsExclusive = tag.IsExclusive,
        IsFallback = tag.IsFallback,
        IsAutoTag = tag.IsAutoTag,
    };
}
