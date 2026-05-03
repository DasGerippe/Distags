using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class TagGroupMapper : IEntityMapper<TagGroup, TagGroupDto>
{
    public Expression<Func<TagGroup, TagGroupDto>> Expression { get; }

    public TagGroupMapper() : this(new TagMapper())
    {
    }

    public TagGroupMapper(IEntityMapper<Tag, TagDto> tagMapper)
    {
        Expression = tagGroup => new TagGroupDto
        {
            Name = tagGroup.Name,
            Description = tagGroup.Description,
            Position = tagGroup.Position,
            Tags = tagGroup.Tags.AsQueryable()
                .OrderBy(tag => tag.Position)
                .Select(tagMapper.Expression)
                .ToList(),
        };
    }
}