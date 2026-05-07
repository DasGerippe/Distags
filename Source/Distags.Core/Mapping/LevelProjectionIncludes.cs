namespace Distags.Core.Mapping;

[Flags]
public enum LevelProjectionIncludes
{
    None = 0,
    Id = 1,
    Name = 2,
    ImageUrl = 4,
    CreatorId = 8,
    CreatorName = 16,
    CreatorAvatarHash = 32,
    Creator = CreatorId | CreatorName | CreatorAvatarHash,
    CreatedOn = 64,
    UpdatedOn = 128,
    FileName = 256,
    FileUrl = 512,
    WorkshopTags = 1024,
    Tags = 2048,
}