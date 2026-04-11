namespace AutoTaggerRunner;

public class AutoTaggerState
{
    public static AutoTaggerState New => new AutoTaggerState
    {
        LastAutoTagging = DateTimeOffset.MinValue,
    };

    public DateTimeOffset LastAutoTagging { get; set; }
}