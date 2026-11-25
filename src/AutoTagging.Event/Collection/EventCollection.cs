namespace AutoTagging.Event.Collection;

public class EventCollection
{
    public required string CollectionFileId { get; set; }

    public required DateTimeOffset CollectionUpdatedOn { get; set; }

    public required string EventTagName { get; set; }
}
