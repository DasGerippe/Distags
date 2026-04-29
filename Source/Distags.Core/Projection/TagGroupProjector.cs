using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Projection;

public class TagGroupProjector : IEntityProjector<TagGroup, TagGroupDto>
{
    public Expression<Func<TagGroup, TagGroupDto>> Projection { get; }

    public TagGroupProjector(IEntityProjector<Tag, TagDto> tagProjector)
    {
        Projection = tagGroup => new TagGroupDto
        {
            Name = tagGroup.Name,
            Description = tagGroup.Description,
            Position = tagGroup.Position,
            Tags = tagGroup.Tags.AsQueryable()
                .OrderBy(tag => tag.Position)
                .Select(tagProjector.Projection)
                .ToList(),
        };
    }
}