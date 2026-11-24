namespace AutoTagging.Abstractions;

public interface ITaggingStrategy
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
