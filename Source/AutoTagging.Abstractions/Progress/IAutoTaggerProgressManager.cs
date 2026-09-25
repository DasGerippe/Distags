namespace AutoTagging.Abstractions.Progress;

public interface IAutoTaggerProgressManager
{
    Task<IAutoTaggerProgress?> LoadProgressAsync(AutoTagger autoTagger);

    Task SaveProgressAsync(AutoTagger autoTagger, IAutoTaggerProgress? progress);
}
