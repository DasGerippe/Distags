using Distags.Contracts;
using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Mapping;

public class LevelProjectionMapper : IEntityMapper<Level, LevelProjectionDto>
{
    public Expression<Func<Level, LevelProjectionDto>> Expression { get; }

    public LevelProjectionMapper(LevelProjectionIncludes includes)
    {
        bool includeId = includes.HasFlag(LevelProjectionIncludes.Id);
        bool includeName = includes.HasFlag(LevelProjectionIncludes.Name);
        bool includeImageUrl = includes.HasFlag(LevelProjectionIncludes.ImageUrl);
        bool includeCreatorId = includes.HasFlag(LevelProjectionIncludes.CreatorId);
        bool includeCreatorName = includes.HasFlag(LevelProjectionIncludes.CreatorName);
        bool includeCreatorAvatarHash = includes.HasFlag(LevelProjectionIncludes.CreatorAvatarHash);
        bool includeCreatedOn = includes.HasFlag(LevelProjectionIncludes.CreatedOn);
        bool includeUpdatedOn = includes.HasFlag(LevelProjectionIncludes.UpdatedOn);
        bool includeFileName = includes.HasFlag(LevelProjectionIncludes.FileName);
        bool includeFileUrl = includes.HasFlag(LevelProjectionIncludes.FileUrl);
        bool includeWorkshopTags = includes.HasFlag(LevelProjectionIncludes.WorkshopTags);
        bool includeTags = includes.HasFlag(LevelProjectionIncludes.Tags);

        Expression = level => new LevelProjectionDto
        {
            Id = includeId ? level.Id : null,
            Name = includeName ? level.Name : null,
            ImageUrl = includeImageUrl ? level.ImageUrl : null,
            CreatorId = includeCreatorId ? level.CreatorId : null,
            CreatorName = includeCreatorName ? level.Creator!.Name : null,
            CreatorAvatarHash = includeCreatorAvatarHash ? level.Creator!.AvatarHash : null,
            CreatedOn = includeCreatedOn ? level.CreatedOn : null,
            UpdatedOn = includeUpdatedOn ? level.UpdatedOn : null,
            FileName = includeFileName ? level.FileName : null,
            FileUrl = includeFileUrl ? level.FileUrl : null,
            WorkshopTags = includeWorkshopTags ? level.WorkshopTags : null,
            Tags = includeTags ? level.Tags.Select(tag => tag.Name).ToList() : null,
        };
    }
}
