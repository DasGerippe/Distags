namespace AutoTagging.Abstractions;

public class TagDelta
{
    public List<string> AddTags { get; set; } = [];

    public List<string> RemoveTags { get; set; } = [];
}
