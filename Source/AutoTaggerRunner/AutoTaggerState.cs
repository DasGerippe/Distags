namespace AutoTaggerRunner;

public class AutoTaggerState
{
    public static AutoTaggerState New { get; } = new AutoTaggerState
    {
        LastAutoTagging = DateTime.MinValue,
    };

    public DateTime LastAutoTagging { get; set; }
}