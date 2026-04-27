using DataClasses;
using Distags.Core.Dtos;
using System.Linq.Expressions;

namespace Distags.Core.Projection;

public class LevelProjector : IEntityProjector<Level, LevelDto>
{
    public Expression<Func<Level, LevelDto>> Projection => level => new LevelDto
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