using System.Linq.Expressions;
using DataClasses;
using Distags.Core.Dtos;

namespace Distags.Core.Mapping;

public class LevelMapper : IEntityMapper<Level, LevelDto>
{
    public Expression<Func<Level, LevelDto>> Expression => level => new LevelDto
    {
        Id = level.Id,
        Name = level.Name,
        ImageUrl = level.IconUrl,
        CreatorId = level.CreatorId,
        CreatorName = level.Creator!.Name,
        CreatorAvatarHash = level.Creator!.AvatarHash,
        CreatedOn = level.CreatedOn,
        UpdatedOn = level.UpdatedOn,
        Tags = level.Tags.Select(t => t.Name).ToList(),
    };
}