namespace DataClasses;

public class TagGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TagCompatibility Compatibility { get; set; } = TagCompatibility.Exclusive;
}