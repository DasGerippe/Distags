namespace DataClasses;

public class TagGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TagCompatibility Compatibility { get; set; } = TagCompatibility.Exclusive;

    public long? ParentGroupId { get; set; }

    public TagGroup? ParentGroup { get; set; }

    public List<TagGroup> SubGroups { get; set; } = [];

    public List<Tag> Tags { get; set; } = [];
}