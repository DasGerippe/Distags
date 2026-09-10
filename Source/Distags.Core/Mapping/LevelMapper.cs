using Distags.Contracts;
using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class LevelMapper : IEntityMapper<Level, LevelDto>
{
    public Expression<Func<Level, LevelDto>> Expression => level => level.Type == LevelType.Official
        ? new OfficialLevelDto
        {
            Slug = (level as OfficialLevel)!.Slug,
            Name = level.Name,
            CreatorName = (level as OfficialLevel)!.CreatorName,
            GameModes = (level as OfficialLevel)!.GameModes,
            Tags = level.Tags.Select(t => t.Name).ToList(),
        }
        : new WorkshopLevelDto
        {
            WorkshopId = (level as WorkshopLevel)!.WorkshopId,
            Name = level.Name,
            CreatorId = (level as WorkshopLevel)!.CreatorId,
            CreatorName = (level as WorkshopLevel)!.Creator!.Name,
            CreatorAvatarHash = (level as WorkshopLevel)!.Creator!.AvatarHash,
            FileName = (level as WorkshopLevel)!.FileName,
            FileUrl = (level as WorkshopLevel)!.FileUrl,
            ImageUrl = (level as WorkshopLevel)!.ImageUrl,
            CreatedOn = (level as WorkshopLevel)!.CreatedOn,
            UpdatedOn = (level as WorkshopLevel)!.UpdatedOn,
            WorkshopTags = (level as WorkshopLevel)!.WorkshopTags,
            Tags = level.Tags.Select(t => t.Name).ToList(),
        };
}
