using AutoTagging.Abstractions.Progress;

namespace AutoTagging.Abstractions;

public class ProgressState
{
    public IAutoTaggerProgress? Progress { get; set; }
}
