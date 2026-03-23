namespace LevelTagApi.Dtos;

public class PutLevelDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public long CreatorId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string IconUrl { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}