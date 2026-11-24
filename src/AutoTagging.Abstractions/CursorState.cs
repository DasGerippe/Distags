using AutoTagging.Abstractions.Progress;

namespace AutoTagging.Abstractions;

public class CursorState
{
    public TimestampLevelIdCursor? Cursor { get; set; }
}
