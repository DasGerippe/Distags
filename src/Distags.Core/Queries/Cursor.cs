namespace Distags.Core.Queries;

public class Cursor
{
    public object Value { get; set; } = new object();

    public long EntityId { get; set; }
}
