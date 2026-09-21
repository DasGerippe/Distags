namespace Distags.Contracts;

public abstract class LevelDto
{
    public long Id { get; set; }

    public abstract LevelType Type { get; }

    public string Name { get; set; } = string.Empty;

    public DateTimeOffset RecordCreatedOn { get; set; }

    public DateTimeOffset RecordUpdatedOn { get; set; }

    public List<string> Tags { get; set; } = [];
}
